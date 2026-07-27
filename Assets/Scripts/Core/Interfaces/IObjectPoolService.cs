using UnityEngine;

namespace Core.Interfaces
{
    public interface IObjectPoolService
    {
        void Register<T>(T prefab, int prewarmCount = 0) where T : Component;
        T Get<T>(Transform parent = null) where T : Component;
        void Return<T>(T instance) where T : Component;
        void Clear<T>() where T : Component;
    }
}