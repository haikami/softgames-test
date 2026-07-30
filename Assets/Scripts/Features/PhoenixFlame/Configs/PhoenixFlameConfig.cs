using System.Collections.Generic;
using Core.Configs;
using UnityEngine;

namespace Features.PhoenixFlame.Configs
{
    [CreateAssetMenu(menuName = "Features/PhoenixFlame/Config")]
    public class PhoenixFlameConfig : FeatureConfig
    {
        [Header("Color Cycle")]
        [Tooltip("Animator state names, in click order. Must match the Animator Controller exactly.")]
        [SerializeField] private string[] _colorStateNames = { "Fire_Orange", "Fire_Green", "Fire_Blue" };
        [SerializeField] private float _colorTransitionDuration = 1.2f;

        public IReadOnlyList<string> ColorStateNames => _colorStateNames;
        public float ColorTransitionDuration => _colorTransitionDuration;
    }
}