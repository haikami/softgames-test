using System;
using System.Collections.Generic;
using System.Threading;
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
    public class MagicWordsController : MonoBehaviour
    {
        [SerializeField] private DialogueLineView _linePrefab;
        [SerializeField] private RectTransform _contentContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private TMP_Text _errorLabel;
        
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

        private void Start()
        {
            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not MagicWordsConfig config)
            {
                Debug.LogError("Current feature is not setup properly: no MagicWordsConfig found.");
                ServiceLocator.Get<IFeatureMenuService>().ReturnToMenu().Forget();
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

        private async UniTaskVoid RunFlow()
        {
            //Cleanup and display loading screen while fetching dialogue data
            _errorPanel.SetActive(false);
            ClearPresentedLines();
            _lifecycleCts = new CancellationTokenSource();
            _loadingScreen.Show(this);

            var sourceResult = await FetchDialogueData();
            _loadingScreen.Hide(this);
            
            if (!sourceResult.IsSuccess)
            {
                _loadingScreen.Hide(this);
                ShowError(sourceResult.Error);
                return;
            }

            //Map and purge the data into more useful structure
            var mapper = new DialogueDataMapper();
            var model = mapper.Map(sourceResult.Value);

            //Give some seconds to load avatars
            await _avatarsLoader.LoadAllWithGrace(model.AvatarsByName, _config.AvatarGraceSeconds);

            _topBarView.SetButtonsVisibility(true, true, true);
            //Start displaying dialogues
            await _dialogueDisplayer.DisplayDialogue(model.Lines, DisplayLine, _lifecycleCts.Token);
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

        private void DisplayLine(DialogueEntryModel line)
        {
            var view = _pool.Get<DialogueLineView>(parent: _contentContainer);
            view.Setup(line); 
            _activeLineViews.Add(view);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentContainer);
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        private void ShowError(NetworkError error)
        {
            _errorLabel.text = error.Type == NetworkErrorType.Http
                ? $"Server error: {error.Message}"
                : "Couldn't load dialogue. Check your connection and try again.";
            _errorPanel.SetActive(true);
        }

        private void ClearPresentedLines()
        {
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