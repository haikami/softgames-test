using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for a simple pool service that can register one prefab per class
    /// </summary>
    public interface IObjectPoolService
    {
        void Register<T>(T prefab, int prewarmCount = 0) where T : Component;
        T Get<T>(Transform parent = null) where T : Component;
        void Return<T>(T instance) where T : Component;
        void Clear<T>() where T : Component;
    }
}