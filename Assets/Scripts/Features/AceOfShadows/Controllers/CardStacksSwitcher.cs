using System;
using System.Collections.Generic;
using Core.Interfaces;
using DG.Tweening;
using Features.AceOfShadows.Animations;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.UI;

namespace Features.AceOfShadows.Controllers
{
    /// <summary>
    /// Moves cards from one stack to the other on a given interval.
    /// Triggers an event after last card reaches destination stack.
    /// </summary>
    public class CardStacksSwitcher
    {
        public event Action OnSequenceCompleted;

        private readonly CardStackController _source;
        private readonly CardStackController _destination;
        private readonly AceOfShadowsConfig _config;
        private readonly CardMoveAnimatorFactory _animatorFactory;
        private readonly List<CardView> _inFlightCards = new();

        public bool IsComplete => _source.Count == 0 && _inFlightCards.Count == 0;

        public CardStacksSwitcher(
            CardStackController source,
            CardStackController destination,
            AceOfShadowsConfig config,
            CardMoveAnimatorFactory animatorFactory)
        {
            _source = source;
            _destination = destination;
            _config = config;
            _animatorFactory = animatorFactory;
        }

        public void MoveNextCard()
        {
            if (_source.Count == 0) return;

            var card = _source.PopTop();
            if (card == null) return;

            _inFlightCards.Add(card);

            var preset = PickPreset();
            var animator = _animatorFactory.Get(preset.Style);

            var worldPos = card.Rect.position;
            card.SetRectParent(_destination.ContentRoot);
            card.Rect.position = worldPos;

            animator.Play(card, _destination.StackTopLocalPosition, preset, () =>
            {
                _inFlightCards.Remove(card);
                _destination.PushTop(card);
                if (IsComplete) OnSequenceCompleted?.Invoke();
            });
        }

        /// Kills any ongoing tweens and return flying cards to the pool.
        public void ClearOngoingFlyingCards(IObjectPoolService pool)
        {
            var cards = new List<CardView>(_inFlightCards);
            foreach (var card in cards)
            {
                DOTween.Kill(card.Rect);
                pool.Return(card);
            }

            _inFlightCards.Clear(); ;
        }

        private CardMoveAnimationPreset PickPreset()
        {
            var presets = _config.CardMovePresets;
            return presets[UnityEngine.Random.Range(0, presets.Count)];
        }
    }
}