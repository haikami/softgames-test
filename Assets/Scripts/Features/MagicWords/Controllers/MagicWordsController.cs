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
        [SerializeField] private Button _retryButton;

        private INetworkService _network;
        private IObjectPoolService _pool;
        private ILoadingScreen _loadingScreen;
        private MagicWordsConfig _config;

        private AvatarsTextureLoader _avatarsLoader;
        private IDialogueDisplayer _dialogueDisplayer;
        private readonly List<DialogueLineView> _activeLineViews = new();
        private CancellationTokenSource _lifecycleCts;

        private void Awake()
        {
            _network = ServiceLocator.Get<INetworkService>();
            _pool = ServiceLocator.Get<IObjectPoolService>();
            _loadingScreen = ServiceLocator.Get<ILoadingScreen>();
            _retryButton.onClick.AddListener(() => RunFlow().Forget());
        }

        private void Start()
        {
            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not MagicWordsConfig config)
            {
                Debug.LogError("Current feature is not setup properly: no MagicWordsConfig found.");
                return;
            }

            _config = config;
            _avatarsLoader = new AvatarsTextureLoader(_network, nameof(MagicWordsController));
            _dialogueDisplayer = new CadenceDialogueDisplayer(_config.TimeBetweenDialogueLines);
            _pool.Register(_linePrefab, _config.InitialDialogBubbles);

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
            _retryButton.enabled = false;
            
            if (!sourceResult.IsSuccess)
            {
                _loadingScreen.Hide(this);
                ShowError(sourceResult.Error);
                _retryButton.enabled = true;
                return;
            }

            //Map and purge the data into more useful structure
            var mapper = new DialogueDataMapper();
            var model = mapper.Map(sourceResult.Value);

            //Give some seconds to load avatars
            await _avatarsLoader.LoadAllWithGrace(model.AvatarsByName, _config.AvatarGraceSeconds);

            _retryButton.enabled = true;
            //Start displaying dialogues
            await _dialogueDisplayer.DisplayDialogue(model.Lines, DisplayLine, _lifecycleCts.Token);
        }

        private async UniTask<Result<IMagicWordsData>> FetchDialogueData()
        {
            if (_config.LocalSourceOverride != null)
                return Result<IMagicWordsData>.Success(_config.LocalSourceOverride);

            var result = await _network.GetJson<MagicWordsResponse>(_config.EndpointUrl, nameof(MagicWordsController));

            return result.IsSuccess
                ? Result<IMagicWordsData>.Success(result.Value)
                : Result<IMagicWordsData>.Failure(result.Error);
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
        }
    }
}