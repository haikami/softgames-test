using System;
using Core.Configs;
using Core.Interfaces;
using Cysharp.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Navigation service that allows going back and forth from each feature.
    /// </summary>
    public class FeatureMenuService : IFeatureMenuService
    {
        private readonly ISceneLoaderService _sceneLoader;
        private bool _isTransitioning;

        public FeatureConfig CurrentFeature { get; private set; }
        public event Action<FeatureConfig> OnFeatureSelected;
        public event Action OnReturnedToMenu;

        public FeatureMenuService(ISceneLoaderService sceneLoader) => _sceneLoader = sceneLoader;

        public async UniTask SelectFeature(FeatureConfig feature)
        {
            if (_isTransitioning || CurrentFeature != null) return;
            _isTransitioning = true;

            await _sceneLoader.LoadAdditive(feature.SceneName);

            CurrentFeature = feature;
            _isTransitioning = false;
            OnFeatureSelected?.Invoke(feature);
        }

        public async UniTask ReturnToMenu()
        {
            if (_isTransitioning || CurrentFeature == null) return;
            _isTransitioning = true;

            await _sceneLoader.UnloadAdditive(CurrentFeature.SceneName);

            CurrentFeature = null;
            _isTransitioning = false;
            OnReturnedToMenu?.Invoke();
        }
    }
}