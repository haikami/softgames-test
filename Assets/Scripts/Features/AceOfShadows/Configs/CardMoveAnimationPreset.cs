using System.Collections.Generic;
using DG.Tweening;
using Features.AceOfShadows.Animations;
using UnityEngine;

namespace Features.AceOfShadows.Configs
{
    [CreateAssetMenu(menuName = "Features/AceOfShadows/Move Animation Preset")]
    public class CardMoveAnimationPreset : ScriptableObject
    {
        [SerializeField] private CardAnimationStyle _style;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private Ease _ease = Ease.InOutQuad;

        [Header("Arc-only")]
        [SerializeField] private float _arcHeight = 80f;

        public CardAnimationStyle Style => _style;
        public float Duration => _duration;
        public Ease Ease => _ease;
        public float ArcHeight => _arcHeight;
    }
}