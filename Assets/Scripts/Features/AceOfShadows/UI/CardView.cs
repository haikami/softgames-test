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

        public void OnSpawned()
        {
            gameObject.SetActive(true);
        }

        public void OnReturned()
        {
            _rect.localRotation = Quaternion.identity;
            _rect.localScale = Vector3.one;
            _rect.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}