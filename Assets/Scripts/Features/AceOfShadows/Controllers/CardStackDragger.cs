using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.AceOfShadows.Controllers
{
    /// <summary>
    /// Allows dragging card stacks along the screen
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardStackDragger : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private RectTransform _rect;

        private Vector2 _startAnchoredPosition;
        private Vector2 _dragOffset;

        private void Awake()
        {
            _rect = (RectTransform)transform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startAnchoredPosition = _rect.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_rect.parent,
                eventData.position,
                eventData.pressEventCamera,
                out var pointerLocal);

            _dragOffset = _rect.anchoredPosition - pointerLocal;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_rect.parent,
                eventData.position,
                eventData.pressEventCamera,
                out var pointerLocal);

            _rect.anchoredPosition = pointerLocal + _dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        { }

        public void ResetPosition()
        {
            _rect.anchoredPosition = _startAnchoredPosition;
        }
    }
}