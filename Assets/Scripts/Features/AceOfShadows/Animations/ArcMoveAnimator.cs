using System;
using DG.Tweening;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.Interfaces;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Animations
{
    /// <summary>
    /// Moves from a to b using an arc of desired height
    /// </summary>
    public class ArcMoveAnimator : ICardMoveAnimator
    {
        public void Play(CardView card, Vector2 target, CardMoveAnimationPreset preset, Action onComplete)
        {
            var start = card.Rect.anchoredPosition;
            var end = target;

            var direction = (end - start).normalized;
            var perpendicular = Vector2.Perpendicular(direction);

            var middle =
                Vector2.Lerp(start, end, 0.5f) +
                perpendicular * preset.ArcHeight;

            DOTween.To(() => 0f, t =>
                {
                    var u = 1f - t;
                    card.Rect.anchoredPosition =
                        u * u * start + 2f * u * t * middle + t * t * end;
                }, 1f, preset.Duration)
                .SetTarget(card.Rect)
                .SetEase(preset.Ease)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}