using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Features.AceOfShadows.Animations;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Controllers
{
    /// <summary>
    /// Coordinates the Ace of Shadows feature by initializing the card stacks,
    /// driving the card transfer sequence, handling UI interactions, and managing
    /// reset and cheat functionality.
    /// </summary>
    public class AceOfShadowsController : MonoBehaviour
    {
        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private CardStackView _sourceStackView;
        [SerializeField] private CardStackView _destinationStackView;
        [SerializeField] private GameObject _sequenceCompleteView;
        
        private const string FasterCheatMessage = "X1";
        private const string SlowerCheatMessage = "X3";
        
        private IObjectPoolService _pool;
        private ITopBarView _topBarView;
        
        private CardStackController _source;
        private CardStackController _destination;
        private CardStacksSwitcher _stacksSwitcher;
        private AceOfShadowsConfig _config;
        private Vector3 _sourcePosition;
        private Vector3 _destinationPosition;
        private float _timer;

        private void Start()
        {

            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not AceOfShadowsConfig config)
            {
                Debug.LogError("Current feature is not setup properly: No suitable config found for feature");
                ServiceLocator.Get<IFeatureMenuService>().ReturnToMenu().Forget();
                return;
            }
            
            _config = config;
            SetupTopBar();
            SetupStacks();
        }

        private void SetupTopBar()
        {
            _topBarView = ServiceLocator.Get<ITopBarView>();
            _topBarView.SetButtonsVisibility(true, true, true);
            _topBarView.OnResetButtonPressed += Reset;
            _topBarView.SetupCheatButton(FasterCheatMessage, GoFasterCheatClicked);
        }
        
        private void GoFasterCheatClicked()
        {
            Time.timeScale = 3f;
            _topBarView?.SetupCheatButton(SlowerCheatMessage, GoSlowerCheatClicked);
        }

        private void GoSlowerCheatClicked()
        {
            Time.timeScale = 1f;
            _topBarView?.SetupCheatButton(FasterCheatMessage, GoFasterCheatClicked);
        }

        /// <summary>
        /// Creates the source and destination stacks, initializes the card pool,
        /// and prepares the sequence controller.
        /// </summary>
        private void SetupStacks()
        {
            _pool = ServiceLocator.Get<IObjectPoolService>();
            _pool.Register(_cardPrefab, _config.TotalCardCount);

            _source = new CardStackController( _sourceStackView.ContentRoot, _config.CardStackOffset);
            _sourceStackView.Bind(_source);
            _source.AddCards(_pool, _config.TotalCardCount);
            _sourcePosition = _sourceStackView.transform.position;
            
            _destination = new CardStackController( _destinationStackView.ContentRoot, _config.CardStackOffset);
            _destinationStackView.Bind(_destination);
            _destinationPosition = _destinationStackView.transform.position;
            
            _stacksSwitcher = new CardStacksSwitcher(_source, _destination, _config, new CardMoveAnimatorFactory());
            _stacksSwitcher.OnSequenceCompleted += ShowSequenceCompleteView;
        }

        private void ShowSequenceCompleteView() => _sequenceCompleteView?.SetActive(true);

        /// <summary>
        /// Advances the card sequence at the configured interval until all cards
        /// have been transferred.
        /// </summary>
        private void Update()
        {
            if (_stacksSwitcher == null || _stacksSwitcher.IsComplete) return;

            _timer += Time.deltaTime;
            if (_timer >= _config.MoveInterval)
            {
                _timer -= _config.MoveInterval;
                _stacksSwitcher.MoveNextCard();
            }
        }

        private void OnDestroy()
        {
            _pool?.Clear<CardView>();

            if (_topBarView != null)
            {
                _topBarView.OnResetButtonPressed -= Reset;
                _topBarView.ClearCheatButton();
            }
            
            Time.timeScale = 1f;
            DOTween.Kill(this); // safety net for any pending tweens
        }

        private void Reset()
        {
            Time.timeScale = 1f;
            
            _timer = 0f;
            _source.Clear(_pool);
            _destination.Clear(_pool);
            _stacksSwitcher.ClearOngoingFlyingCards(_pool);
            
            _source.AddCards(_pool, _config.TotalCardCount);
            
            _sourceStackView.transform.position = _sourcePosition;
            _destinationStackView.transform.position = _destinationPosition;
            
            _topBarView?.SetupCheatButton(FasterCheatMessage, GoFasterCheatClicked);
            
            _sequenceCompleteView?.SetActive(false);
        }
    }
}