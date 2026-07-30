using Core.Interfaces;
using UnityEngine;

namespace Features.AceOfShadows.UI
{

    /// <summary>
    /// Main component of the card view, owns card rect transform.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardView : MonoBehaviour, IPoolable
    {
        [SerializeField] private RectTransform _rect;

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