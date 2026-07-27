using Core.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Buttons
{
    public class BackToMenuButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
                ServiceLocator.Get<IFeatureMenuService>().ReturnToMenu().Forget());
        }
    }
}