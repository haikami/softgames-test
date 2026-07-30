using Core.Interfaces;
using UnityEngine;

namespace Core.Tests.EditMode.Services
{
    /// <summary>
    /// Minimal stand-in for a pooled RectTransform-based view (mirrors CardView's
    /// OnReturned behaviour), used to exercise ObjectPoolService in isolation from
    /// any real prefab, canvas, or scene.
    /// </summary>
    public class PoolableRect : MonoBehaviour, IPoolable
    {
        public int SpawnedCount { get; private set; }
        public int ReturnedCount { get; private set; }
        public RectTransform Rect { get; private set; }

        private void Awake() => Rect = (RectTransform)transform;

        public void OnSpawned()
        {
            SpawnedCount++;
            gameObject.SetActive(true);
        }

        public void OnReturned()
        {
            ReturnedCount++;
            Rect.localRotation = Quaternion.identity;
            Rect.localScale = Vector3.one;
            Rect.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
