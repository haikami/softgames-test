using System;
using Core.Configs;
using Core.Interfaces;
using Cysharp.Threading.Tasks;

namespace Core.Services
{
    public class FeatureMenuService : IFeatureMenuService
    {
        private readonly ISceneLoaderService _sceneLoader;
        private bool _isTransitioning;

        public FeatureDefinition CurrentFeature { get; private set; }
        public event Action<FeatureDefinition> OnFeatureSelected;
        public event Action OnReturnedToMenu;

        public FeatureMenuService(ISceneLoaderService sceneLoader) => _sceneLoader = sceneLoader;

        public async UniTask SelectFeature(FeatureDefinition feature)
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