using Core.Interfaces;
using Features.MagicWords.Enums;
using Features.MagicWords.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MagicWords.UI
{
    /// <summary>
    /// Displays a bubble speech and an avatar view, can have different setups depending
    /// on the position of the avatar.
    /// Can be pooled
    /// </summary>
    public class DialogueLineView : MonoBehaviour, IPoolable
    {
        [Header("References")]
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private RectTransform _avatarSlot;
        [SerializeField] private RectTransform _bubbleSlot;
        [SerializeField] private AvatarView _avatarView;
        [SerializeField] private TMP_Text _textLabel;
        [SerializeField] private Image _bubbleImage;
        
        [Header("Settings")]
        [SerializeField] private Color _leftDialogueBubbleColor;
        [SerializeField] private Color _rightDialogueBubbleColor;
        
        public void Setup(DialogueEntryModel line)
        {
            _textLabel.text = line.FormattedText;
            SetSide(line.Avatar?.Position ?? AvatarPosition.Left);
            _avatarView.Bind(line.Avatar);
            _avatarView.SetupName(line.SpeakerName);
            transform.localScale = Vector3.one;
        }

        private void SetSide(AvatarPosition position)
        {
            var isRight = position == AvatarPosition.Right;
            _avatarSlot.SetSiblingIndex(isRight ? 1 : 0);
            _bubbleSlot.SetSiblingIndex(isRight ? 0 : 1);
            _bubbleImage.color = isRight ? _rightDialogueBubbleColor : _leftDialogueBubbleColor;
            _layoutGroup.childAlignment = isRight ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
        }

        public void OnSpawned() => gameObject.SetActive(true);

        public void OnReturned()
        {
            if (this == null) return;
            
            gameObject.SetActive(false);
            _avatarView.Unbind();
        }
    }
}