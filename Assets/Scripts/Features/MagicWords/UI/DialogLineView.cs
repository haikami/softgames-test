using Core.Interfaces;
using Features.MagicWords.Enums;
using Features.MagicWords.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MagicWords.UI
{
    public class DialogueLineView : MonoBehaviour, IPoolable
    {
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private RectTransform _avatarSlot;
        [SerializeField] private RectTransform _bubbleSlot;
        [SerializeField] private AvatarView _avatarView;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _textLabel;

        public AvatarView AvatarView => _avatarView;

        public void Setup(DialogueEntryModel line)
        {
            _nameLabel.text = line.SpeakerName;
            _textLabel.text = line.FormattedText;
            SetSide(line.Avatar?.Position ?? AvatarPosition.Left);
            _avatarView.Bind(line.Avatar);
        }

        private void SetSide(AvatarPosition position)
        {
            var isRight = position == AvatarPosition.Right;
            _avatarSlot.SetSiblingIndex(isRight ? 1 : 0);
            _bubbleSlot.SetSiblingIndex(isRight ? 0 : 1);
            _layoutGroup.childAlignment = isRight ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
        }

        public void OnSpawned() => gameObject.SetActive(true);

        public void OnReturned()
        {
            gameObject.SetActive(false);
            _avatarView.Unbind(); 
        }
    }
}