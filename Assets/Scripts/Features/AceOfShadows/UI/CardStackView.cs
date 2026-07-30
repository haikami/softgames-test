using Features.AceOfShadows.Controllers;
using TMPro;
using UnityEngine;

namespace Features.AceOfShadows.UI
{
    /// <summary>
    /// Displays the amount of cards of a stack listening
    /// to the events of the stack controller
    /// </summary>
    public class CardStackView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _counterLabel;

        private CardStackController _cardStackController;

        public RectTransform ContentRoot => _content;

        public void Bind(CardStackController cardStackController)
        {
            _cardStackController = cardStackController;
            _cardStackController.OnCardAdded +=RefreshCounter;
            _cardStackController.OnCardRemoved +=RefreshCounter;
            RefreshCounter();
        }

        private void RefreshCounter(CardView cardView = null) => _counterLabel.text = _cardStackController.Count.ToString();

        private void OnDestroy()
        {
            if (_cardStackController == null) return;
            _cardStackController.OnCardAdded -= RefreshCounter;
            _cardStackController.OnCardRemoved -= RefreshCounter;
        }
    }
}