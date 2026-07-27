using System.Collections.Generic;
using Core;
using Core.Interfaces;
using DG.Tweening;
using Features.AceOfShadows.Animations;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.Controllers;
using Features.AceOfShadows.Services;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows
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
            _pool = ServiceLocator.Get<IObjectPoolService>();

            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not AceOfShadowsConfig config)
            {
                Debug.LogError("No config found for feature");
                //TODO: handle error better
                return;
            }

            _config = config;
            _pool.Register(_cardPrefab, _config.TotalCardCount);

            _source = new CardStackController( _sourceStackView.ContentRoot, _config.CardStackOffset);
            _destination = new CardStackController( _destinationStackView.ContentRoot, _config.CardStackOffset);
            _sourceStackView.Bind(_source);
            _destinationStackView.Bind(_destination);

            DealInitialCards();

            _stacksSwitcher = new CardStacksSwitcher(_source, _destination, _config, new CardMoveAnimatorFactory());
            _stacksSwitcher.OnSequenceCompleted += ShowBanner;
        }

        private void ShowBanner() => _banner?.SetActive(true);

        private void DealInitialCards()
        {
            for (var i = 0; i < _config.TotalCardCount; i++)
            {
                var card = _pool.Get<CardView>();
                _source.PushTop(card);
            }
        }

        private void Update()
        {
            if (_stacksSwitcher == null || _stacksSwitcher.IsComplete) return;

            _timer += Time.deltaTime;
            if (_timer >= _config.MoveInterval)
            {
                _timer = 0f;
                _stacksSwitcher.MoveNextCard();
            }
        }

        private void OnDestroy()
        {
            _pool.Clear<CardView>();
            DOTween.Kill(this); // safety net for any pending tweens
        }
    }
}