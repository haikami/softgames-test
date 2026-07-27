using System;
using Core.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Buttons
{
    public class FeatureMenuButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        public void Init(FeatureDefinition feature, Action<FeatureDefinition> onClick)
        {
            _label.text = feature.DisplayName;
            _button.onClick.AddListener(() => onClick(feature));
        }
    }
}