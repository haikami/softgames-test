using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Core.Networking;
using Cysharp.Threading.Tasks;
using Features.MagicWords.Configs;
using Features.MagicWords.Interfaces;
using Features.MagicWords.Models;
using Features.MagicWords.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MagicWords.Controllers
{
    
    /// <summary>
    /// Coordinates the Magic Words feature by loading dialogue data, downloading
    /// avatars, displaying the conversation, and managing possible errors.
    /// </summary>
    public class MagicWordsController : MonoBehaviour
    {
        [SerializeField] private DialogueLineView _linePrefab;
        [SerializeField] private RectTransform _contentContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private TMP_Text _errorLabel;

        private const float ErrorMessageDisplaySeconds = 3f;
        private bool _forceOverrideWithLocalConfigToggle;

        private INetworkService _network;
        private IObjectPoolService _pool;
        private ILoadingScreen _loadingScreen;
        private ITopBarView _topBarView;
        private MagicWordsConfig _config;

        private AvatarsTextureLoader _avatarsLoader;
        private IDialogueDisplayer _dialogueDisplayer;
        private readonly List<DialogueLineView> _activeLineViews = new();
        private CancellationTokenSource _lifecycleCts;

        private void Awake()
        {
            SetupServices();
            SetupDebugButtons();
        }

        /// <summary>
        /// Validates the feature configuration, initializes dependencies, and starts
        /// the dialogue loading flow.
        /// </summary>
        private void Start()
        {
            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not MagicWordsConfig config)
            {
                var message = "Current feature is not setup properly: no MagicWordsConfig found.";
                Debug.LogError(message);
                ShowErrorAndReturn(message).Forget();
                return;
            }

            _config = config;
            _avatarsLoader = new AvatarsTextureLoader(_network, nameof(MagicWordsController));
            _dialogueDisplayer = new CadenceDialogueDisplayer(_config.TimeBetweenDialogueLines);
            _pool.Register(_linePrefab, _config.InitialDialogBubbles);
            RunFlow().Forget();
        }
        
        
        private void SetupServices()
        {
            _network = ServiceLocator.Get<INetworkService>();
            _pool = ServiceLocator.Get<IObjectPoolService>();
            _loadingScreen = ServiceLocator.Get<ILoadingScreen>();
            _topBarView = ServiceLocator.Get<ITopBarView>();
        }
        
        private void SetupDebugButtons()
        {
            _topBarView.OnResetButtonPressed += RunFlowAndForget;
            _topBarView.SetupCheatButton(
                RefreshForceDataSourceButtonText,
                ToggleDataFetchSource);
        }
        
        private void ToggleDataFetchSource()
        {
            _forceOverrideWithLocalConfigToggle = !_forceOverrideWithLocalConfigToggle;
            _topBarView.SetupCheatButton(RefreshForceDataSourceButtonText, ToggleDataFetchSource);
            RunFlowAndForget();
        }

        private string RefreshForceDataSourceButtonText => _forceOverrideWithLocalConfigToggle 
                ? "fetch backend"
                : "try local";


        private void RunFlowAndForget()
        {
            RunFlow().Forget();
        }

        /// <summary>
        /// Executes the complete dialogue flow: loads the dialogue data, downloads
        /// avatars, displays the conversation, and handles any loading errors.
        /// </summary>
        private async UniTaskVoid RunFlow()
        {
            //Cleanup and display loading screen while fetching dialogue data
            _errorPanel.SetActive(false);
            ClearPresentedLines();
            _lifecycleCts = new CancellationTokenSource();
            _loadingScreen.Show(this);
            try
            {
                //Fetch dialogue
                var sourceResult = await FetchDialogueData();

                if (!sourceResult.IsSuccess)
                {
                    throw new DialogueLoadingException(sourceResult.Error.Message);
                }

                //Map dialogue lines with avatars
                var mapper = new DialogueDataMapper();
                var model = mapper.Map(sourceResult.Value);

                if (!model.HasDialogues)
                {
                    throw new DialogueLoadingException("No dialogue lines were returned.");
                }

                //Load all avatars and give a grace period before starting to show dialogues if some avatar hasn't loaded
                await _avatarsLoader.LoadAllWithGrace(
                    model.AvatarsByName,
                    _config.AvatarGraceSeconds);

                _loadingScreen.Hide(this);
                _topBarView.SetButtonsVisibility(true, true, true);

                //Start displaying dialogue
                await _dialogueDisplayer.DisplayDialogue(
                    model.Lines,
                    DisplayLine,
                    _lifecycleCts.Token);
            }
            //If any exception happens during dialogue loading, display an error for several seconds
            catch (DialogueLoadingException e)
            {
                ShowErrorAndReturn(e.Message).Forget();
            }
        }

        private async UniTask<Result<IMagicWordsData>> FetchDialogueData()
        {
            if (_forceOverrideWithLocalConfigToggle)
            {
                return await FetchFakeData();
            }

            var result = await _network.GetJson<MagicWordsResponse>(_config.EndpointUrl, nameof(MagicWordsController));

            return result.IsSuccess
                ? Result<IMagicWordsData>.Success(result.Value)
                : Result<IMagicWordsData>.Failure(result.Error);
        }

        private async UniTask<Result<IMagicWordsData>> FetchFakeData()
        {
            var localConfig = _config.LocalConfigOverride;
            if (localConfig.FakeFetchDelay > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(localConfig.FakeFetchDelay));
            }

            return _config.OverrideWithLocalConfigAvailable 
                ? Result<IMagicWordsData>.Success(_config.LocalConfigOverride) 
                : Result<IMagicWordsData>.Failure(NetworkError.Unreachable("No fake data found in config, setup one first."));
        }

        /// <summary>
        /// Creates and displays a dialogue bubble, then scrolls the conversation to
        /// the latest message.
        /// </summary>
        private void DisplayLine(DialogueEntryModel line)
        {
            var view = _pool.Get<DialogueLineView>(parent: _contentContainer);
            view.Setup(line); 
            _activeLineViews.Add(view);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentContainer);
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        private async UniTask ShowErrorAndReturn(string message)
        {
            _loadingScreen.Hide(this);
            ShowError($"Error: {message}\nGoing back to main menu in {ErrorMessageDisplaySeconds} seconds..");
            await Task.Delay(TimeSpan.FromSeconds(ErrorMessageDisplaySeconds));
            ServiceLocator.Get<IFeatureMenuService>().ReturnToMenu().Forget();
        }

        private void ShowError(string message)
        {
            _errorLabel.text = message;
            _errorPanel.SetActive(true);
        }

        /// <summary>
        /// Returns all active dialogue views to the pool and cancels any ongoing
        /// dialogue presentation.
        /// </summary>
        private void ClearPresentedLines()        {
            foreach (var view in _activeLineViews)
                _pool.Return(view);
            _activeLineViews.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentContainer);
            _lifecycleCts?.Cancel();
        }

        private void OnDestroy()
        {
            _avatarsLoader?.CancelAll();
            _network?.CancelAll(nameof(MagicWordsController));

            ClearPresentedLines();
            _lifecycleCts?.Dispose();
            _pool?.Clear<DialogueLineView>();

            if (_topBarView != null)
            {
                _topBarView.OnResetButtonPressed -= RunFlowAndForget;
                _topBarView.ClearCheatButton();
            }
        }
    }
}