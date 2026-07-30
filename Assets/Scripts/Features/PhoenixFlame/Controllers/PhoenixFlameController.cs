using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Features.PhoenixFlame.Configs;
using UnityEngine;

namespace Features.PhoenixFlame.Controllers
{
    public class PhoenixFlameController : MonoBehaviour
    {
        [SerializeField] private FlameController _flameController;

        private ITopBarView _topBarView;
        private PhoenixFlameConfig _config;

        private void Awake()
        {
            _topBarView = ServiceLocator.Get<ITopBarView>();
            _topBarView.OnResetButtonPressed += Reset;
        }

        private void Start()
        {
            if (ServiceLocator.Get<IFeatureMenuService>().CurrentFeature is not PhoenixFlameConfig config)
            {
                Debug.LogError("Current feature is not setup properly: no PhoenixFlameConfig found.");
                ReturnToMenu();
                return;
            }

            _config = config;

            if (!_flameController.Initialize(_config.ColorStateNames, _config.ColorTransitionDuration))
            {
                ReturnToMenu();
                return;
            }

            _flameController.Play();

            _topBarView.SetupCheatButton(NextColorButtonLabel, AdvanceColor);
            _topBarView.SetButtonsVisibility(true, true, true);
        }

        private void AdvanceColor()
        {
            _flameController.AdvanceColor();
            // refresh label to show the new "next" preview
            _topBarView.SetupCheatButton(NextColorButtonLabel, AdvanceColor); 
        }

        private string NextColorButtonLabel => $"To {_flameController.NextColorState.Replace("Fire_","")}";

        private void Reset()
        {
            if (_config == null) return;

            _flameController.ResetFlame();
            _topBarView.SetupCheatButton(NextColorButtonLabel, AdvanceColor);
        }

        private void ReturnToMenu() => ServiceLocator.Get<IFeatureMenuService>().ReturnToMenu().Forget();

        private void OnDestroy()
        {
            if (_topBarView == null) return;
            
            _topBarView.OnResetButtonPressed -= Reset;
            _topBarView.ClearCheatButton();
        }
    }
}