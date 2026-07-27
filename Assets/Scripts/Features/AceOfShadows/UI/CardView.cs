using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Features.AceOfShadows.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CardView : MonoBehaviour, IPoolable
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Image _image;

        public RectTransform Rect => _rect;

        public void SetRectParent(RectTransform parent) => Rect.SetParent(parent, false);
        public void ClearRectParent() => SetRectParent(null);

        public void OnSpawned()
        {
            gameObject.SetActive(true);
            _rect.localRotation = Quaternion.identity;
            _rect.localScale = Vector3.one;
        }

        public void OnReturned() => gameObject.SetActive(false);
    }
}