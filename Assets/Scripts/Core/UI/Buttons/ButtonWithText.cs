using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Buttons
{
    public class ButtonWithText : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        public void Setup(string text, Action onClick)
        {
            _text.text = text;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(()=>onClick?.Invoke());
        }
    }
}