using System;
using Core.Configs;
using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for menu. Used for navigating through features.
    /// </summary>
    public interface IFeatureMenuService
    {
        FeatureConfig CurrentFeature { get; }
        event Action<FeatureConfig> OnFeatureSelected;
        event Action OnReturnedToMenu;

        UniTask SelectFeature(FeatureConfig feature);
        UniTask ReturnToMenu();
    }
}