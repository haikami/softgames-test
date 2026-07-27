using System;
using Features.AceOfShadows.Configs;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Interfaces
{
    public interface ICardMoveAnimator
    {
        void Play(CardView card, Vector2 target, CardMoveAnimationPreset preset, Action onComplete);
    }
}