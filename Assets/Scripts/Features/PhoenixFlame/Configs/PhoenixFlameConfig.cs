using Core.Configs;
using UnityEngine;

namespace Features.PhoenixFlame.Configs
{
    [CreateAssetMenu(menuName = "Features/PhoenixFlame/Config")]
    public class PhoenixFlameConfig : FeatureConfig
    {
        [Header("Settings")]
        [SerializeField] private bool _autoPlayOnEnter = true;
        [Tooltip("Must match the Animator Controller's entry state name exactly.")]
        [SerializeField] private string _initialColorStateName = "Fire_Orange";

        public bool AutoPlayOnEnter => _autoPlayOnEnter;
        public string InitialColorStateName => _initialColorStateName;
    }
}