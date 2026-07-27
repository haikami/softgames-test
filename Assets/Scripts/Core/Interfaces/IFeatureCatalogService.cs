using System.Collections.Generic;
using Core.Configs;

namespace Core.Interfaces
{
    public interface IFeatureCatalogService
    {
        IReadOnlyList<FeatureDefinition> Features { get; }
    }
}