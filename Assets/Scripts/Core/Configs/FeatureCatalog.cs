using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Core.Configs
{
    /// <summary>
    /// Contains the list of available features.
    /// </summary>
    [CreateAssetMenu(menuName = "Features/Feature Catalog")]
    public class FeatureCatalog : ScriptableObject, IFeatureCatalogService
    {
        [SerializeField] private List<FeatureConfig> _features;
        public IReadOnlyList<FeatureConfig> Features => _features;
    }
}