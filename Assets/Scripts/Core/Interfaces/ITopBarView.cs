using System;

namespace Core.Interfaces
{
    public interface ITopBarView
    {
        event Action OnBackButtonPressed;
        event Action OnResetButtonPressed;
        void SetButtonsVisibility(bool backButtonVisible = false, bool resetButtonVisible = false, bool cheatButtonVisible = false);
        
        void SetupCheatButton(string cheatName, Action onCheatButtonPressed);
        void ClearCheatButton();
    }
}