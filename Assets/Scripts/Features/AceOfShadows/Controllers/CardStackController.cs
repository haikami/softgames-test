using System;
using System.Collections.Generic;
using Core.Interfaces;
using Features.AceOfShadows.UI;
using UnityEngine;

namespace Features.AceOfShadows.Controllers
{
    /// <summary>
    /// Allows pushing and popping from a stack of cards.
    /// Triggers events when cards are added/removed
    /// </summary>
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
        
        public void AddCards(IObjectPoolService pool, int numCards)
        {
            for (var i = 0; i < numCards; i++)
            {
                var card = pool.Get<CardView>();
                PushTop(card);
            }
        }

        public void Clear(IObjectPoolService pool)
        {
            while (_cards.Count > 0)
            {
                pool.Return(PopTop());
            }
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