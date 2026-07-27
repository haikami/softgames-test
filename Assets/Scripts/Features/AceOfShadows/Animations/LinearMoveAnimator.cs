using System;
using DG.Tweening;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.Interfaces;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Animations
{
    public class LinearMoveAnimator : ICardMoveAnimator
    {
        public void Play(CardView card, Vector2 target, CardMoveAnimationPreset preset, Action onComplete)
        {
            card.Rect.DOAnchorPos(target, preset.Duration)
                .SetEase(preset.Ease)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}