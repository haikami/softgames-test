using Core.Configs;
using Core.Interfaces;
using Core.Services;
using UnityEngine;

namespace Core
{
    public class PreloadBootstrapper : MonoBehaviour
    {
        [SerializeField] private FeatureCatalog _featureCatalog;
        [SerializeField] private SceneField _mainMenuScene;

        private async void Start()
        {
            var sceneLoader = new SceneLoaderService();
            var featureMenu = new FeatureMenuService(sceneLoader);

            ServiceLocator.Register<ISceneLoaderService>(sceneLoader);
            ServiceLocator.Register<IFeatureMenuService>(featureMenu);
            ServiceLocator.Register<IFeatureCatalogService>(_featureCatalog);

            await sceneLoader.LoadAdditive(_mainMenuScene.SceneName);
        }
    }
}