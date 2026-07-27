using System;
using System.Collections.Generic;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Controllers
{
    public class CardStackController
    {
        public RectTransform ContentRoot { get; }
        public int Count => _cards.Count;

        public event Action<CardView> OnCardAdded;
        public event Action<CardView> OnCardRemoved;

        private readonly Stack<CardView> _cards = new();
        private readonly float _cardsOffset;

        public CardStackController(RectTransform contentRoot, float cardsOffset)
        {
            _cardsOffset = cardsOffset;
            ContentRoot = contentRoot;
        }

        public CardView PopTop()
        {
            if (_cards.Count == 0) return null;

            var top = _cards.Pop();
            OnCardRemoved?.Invoke(top);
            return top;
        }

        public void PushTop(CardView card)
        {
            card.SetRectParent(ContentRoot);
            card.Rect.anchoredPosition = StackTopLocalPosition;
            _cards.Push(card);
            OnCardAdded?.Invoke(card);
        }

        public Vector2 StackTopLocalPosition => Vector2.up * (_cardsOffset * _cards.Count);
    }
}