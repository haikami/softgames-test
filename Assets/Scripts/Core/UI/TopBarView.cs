using System;
using Core.Interfaces;
using Core.UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    /// <summary>
    /// Top bar, available across features.
    /// Contains debug and navigation buttons.
    /// </summary>
    public class TopBarView : MonoBehaviour, ITopBarView
    {
        
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _backButton;
        //Only one cheat button per feature allowed, purely for the sake of simplicity
        [SerializeField] private ButtonWithText _cheatButton;
        
        public event Action OnBackButtonPressed;
        public event Action OnResetButtonPressed;
        
        private void Awake()
        {
            _resetButton.onClick.AddListener(() => OnResetButtonPressed?.Invoke());
            _backButton.onClick.AddListener(() => OnBackButtonPressed?.Invoke());
        }

        public void SetButtonsVisibility(bool backButtonVisible = false, bool resetButtonVisible = false, bool cheatButtonVisible = false)
        {
            SetObjectVisibility(_backButton, backButtonVisible);
            SetObjectVisibility(_resetButton, resetButtonVisible);
            SetObjectVisibility(_cheatButton, cheatButtonVisible);
        }

        public void SetupCheatButton(string cheatName, Action onCheatButtonPressed) => _cheatButton.Setup(cheatName, onCheatButtonPressed);

        public void ClearCheatButton() => SetupCheatButton(string.Empty, null);
        private static void SetObjectVisibility(Component component, bool isVisible) => component.gameObject.SetActive(isVisible);
    }
}