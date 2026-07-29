using System;
using Core.Configs;
using Core.Interfaces;
using Core.Networking;
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
        //Used for UI elements that must always go on top of everything else
        [SerializeField] private Canvas _persistentCanvas;
        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private TopBarView _topBarView;
        
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
            _topBarView.SetButtonsVisibility(false,false, false);
            //Setup and register prefabs
            DontDestroyOnLoad(_persistentCanvas.gameObject);
            ServiceLocator.Register<ILoadingScreen>(_loadingView);
            ServiceLocator.Register<ITopBarView>(_topBarView);
            

            //Network
            var webRequester = new UnityWebRequester();
            var networkService = new NetworkService(webRequester);
            ServiceLocator.Register<INetworkService>(networkService);
            
            //Rest of common services
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