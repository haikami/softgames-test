using Core.Configs;
using Core.Interfaces;
using Core.UI.Buttons;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// Adds buttons for each feature, hides/shows itself and top bar when entering/exiting features.
    /// </summary>
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private FeatureMenuButton _buttonPrefab;
        [SerializeField] private GameObject _menuRoot;

        private IFeatureMenuService _featureMenu;

        private void Awake()
        {
            _featureMenu = ServiceLocator.Get<IFeatureMenuService>();
            var catalog = ServiceLocator.Get<IFeatureCatalogService>();

            foreach (var feature in catalog.Features)
            {
                var button = Instantiate(_buttonPrefab, _buttonContainer);
                button.Init(feature, OnFeatureButtonClicked);
            }

            _featureMenu.OnFeatureSelected += _ =>
            {
                _menuRoot.SetActive(false);
            };
            _featureMenu.OnReturnedToMenu += () =>
            {
                var topBarView = ServiceLocator.Get<ITopBarView>();
                topBarView.SetButtonsVisibility();
                _menuRoot.SetActive(true);
            };
        }

        private void OnFeatureButtonClicked(FeatureConfig feature)
        {
            _featureMenu.SelectFeature(feature).Forget();
        }
    }
}