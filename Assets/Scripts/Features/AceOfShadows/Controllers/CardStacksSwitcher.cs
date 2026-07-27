using System;
using Features.AceOfShadows.Animations;
using Features.AceOfShadows.Configs;
using UnityEngine;

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

        public bool IsComplete => _source.Count == 0;

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
            if (IsComplete) return;

            var card = _source.PopTop();
            if (card == null) 
            {
                Debug.LogError("This should only happen if there is a null card in the stack.");
                return;  
            }

            var preset = PickPreset();
            var animator = _animatorFactory.Get(preset.Style);
            
            var worldPos = card.Rect.position;
            card.SetRectParent(_destination.ContentRoot);
            card.Rect.position = worldPos;

            animator.Play(card, _destination.StackTopLocalPosition, preset, () =>
            {
                _destination.PushTop(card);
                if (IsComplete)
                {
                    OnSequenceCompleted?.Invoke();
                }
            });
        }

        private CardMoveAnimationPreset PickPreset()
        {
            var presets = _config.CardMovePresets;
            return presets[UnityEngine.Random.Range(0, presets.Count)];
        }
    }
}