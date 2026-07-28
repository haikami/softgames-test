using System;
using Core.Configs;
using Core.Interfaces;
using Core.Services;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class PreloadBootstrapper : MonoBehaviour
    {
        [SerializeField] private FeatureCatalog _featureCatalog;
        [SerializeField] private SceneField _mainMenuScene;

        [Header("References")]
        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private FpsCounterView _fpsCounterView;
        
        private async void Start()
        {
            try
            {
                await Setup();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask Setup()
        {
            _loadingView.Show();
            
            DontDestroyOnLoad(_loadingView);
            DontDestroyOnLoad(_fpsCounterView);
            ServiceLocator.Register<ILoadingScreen>(_loadingView);
            
            var sceneLoader = new SceneLoaderService();
            var featureMenu = new FeatureMenuService(sceneLoader);

            ServiceLocator.Register<ISceneLoaderService>(sceneLoader);
            ServiceLocator.Register<IFeatureMenuService>(featureMenu);
            ServiceLocator.Register<IFeatureCatalogService>(_featureCatalog);
            ServiceLocator.Register<IObjectPoolService>(new ObjectPoolService());

            await sceneLoader.LoadAdditive(_mainMenuScene.SceneName);
            
            _loadingView.Hide();
        }
    }
}