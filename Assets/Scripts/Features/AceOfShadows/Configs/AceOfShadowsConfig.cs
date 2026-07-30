using System.Collections.Generic;
using Core.Configs;
using UnityEngine;

namespace Features.AceOfShadows.Configs
{
    [CreateAssetMenu(menuName = "Features/AceOfShadows/Config")]
    public class AceOfShadowsConfig : FeatureConfig
    {
        [Space(10)]
        [Header("Additional Settings")]
        [SerializeField] private float _moveInterval = 1f;
        [SerializeField] private int _totalCardCount = 144;
        
        [Tooltip("vertical offset between cards in the stack")]
        [SerializeField] private float _cardStackOffset = 2f;
        [SerializeField] private List<CardMoveAnimationPreset> _cardMovePresets;

        public int TotalCardCount => _totalCardCount;
        public float MoveInterval => _moveInterval;
        public IReadOnlyList<CardMoveAnimationPreset> CardMovePresets => _cardMovePresets;
        public float CardStackOffset => _cardStackOffset;
    }
}