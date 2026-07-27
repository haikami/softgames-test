using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(menuName = "Features/Feature Catalog")]
    public class FeatureCatalog : ScriptableObject, IFeatureCatalogService
    {
        [SerializeField] private List<FeatureDefinition> _features;
        public IReadOnlyList<FeatureDefinition> Features => _features;
    }
}