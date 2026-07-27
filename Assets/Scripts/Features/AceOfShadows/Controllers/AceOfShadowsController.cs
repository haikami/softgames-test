using Core;
using Core.Interfaces;
using DG.Tweening;
using Features.AceOfShadows.Animations;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Controllers
{
    public class AceOfShadowsController : MonoBehaviour
    {
        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private CardStackView _sourceStackView;
        [SerializeField] private CardStackView _destinationStackView;
        [SerializeField] private GameObject _banner;

        private IObjectPoolService _pool;
        private CardStackController _source;
        private CardStackController _destination;
        private CardStacksSwitcher _stacksSwitcher;
        private AceOfShadowsConfig _config;
        private float _timer;

        private void Start()
        {

            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not AceOfShadowsConfig config)
            {
                Debug.LogError("Current feature is not setup properly: No suitable config found for feature");
                //TODO: handle error better
                return;
            }
            _config = config;
            SetupStacks();
        }

        private void SetupStacks()
        {
            _pool = ServiceLocator.Get<IObjectPoolService>();
            _pool.Register(_cardPrefab, _config.TotalCardCount);

            _source = new CardStackController( _sourceStackView.ContentRoot, _config.CardStackOffset);
            _sourceStackView.Bind(_source);
            _source.AddCards(_pool, _config.TotalCardCount);
            
            _destination = new CardStackController( _destinationStackView.ContentRoot, _config.CardStackOffset);
            _destinationStackView.Bind(_destination);

            _stacksSwitcher = new CardStacksSwitcher(_source, _destination, _config, new CardMoveAnimatorFactory());
            _stacksSwitcher.OnSequenceCompleted += ShowBanner;
        }

        private void ShowBanner() => _banner?.SetActive(true);

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
            if (_pool != null)
            {
                _pool.Clear<CardView>();
            }
            DOTween.Kill(this); // safety net for any pending tweens
        }
    }
}