using System;
using Core.Configs;
using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFeatureMenuService
    {
        FeatureDefinition CurrentFeature { get; }
        event Action<FeatureDefinition> OnFeatureSelected;
        event Action OnReturnedToMenu;

        UniTask SelectFeature(FeatureDefinition feature);
        UniTask ReturnToMenu();
    }
}