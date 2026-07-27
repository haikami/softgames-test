using System;
using Core.Configs;
using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFeatureMenuService
    {
        FeatureConfig CurrentFeature { get; }
        event Action<FeatureConfig> OnFeatureSelected;
        event Action OnReturnedToMenu;

        UniTask SelectFeature(FeatureConfig feature);
        UniTask ReturnToMenu();
    }
}