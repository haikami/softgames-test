using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Core.Interfaces
{
    public interface IObjectPoolService
    {
        T Get<T>(T prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component;
        void Return<T>(T instance) where T : Component;
        void Prewarm<T>(T prefab, int count) where T : Component;
        void Clear();
    }
}