using Features.MagicWords.Enums;
using Features.MagicWords.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MagicWords.UI
{
    public class AvatarView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private GameObject _readyContent;
        [SerializeField] private GameObject _pendingContent;
        [SerializeField] private GameObject _defaultContent;

        private AvatarModel _model;

        public void SetupName(string avatarName) => _name.text = avatarName;

        public void Bind(AvatarModel model)
        {
            Unbind();
            _model = model;
            if (_model == null)
            {
                UpdateAvatarState(AvatarVisualState.NotSetup);
                return;
            }

            _model.OnTextureSet += HandleTextureSet;
            _model.OnTextureFailed += UpdateAvatarState;
            UpdateAvatarState();
        }

        private void UpdateAvatarState() => UpdateAvatarState(_model.State);
        
        private void UpdateAvatarState(AvatarVisualState state)
        {
            if (state == AvatarVisualState.Ready)
            {
                _avatarImage.sprite = ToSprite(_model.Texture);
            }
            
            _readyContent.SetActive(state == AvatarVisualState.Ready);
            _defaultContent.SetActive(state is AvatarVisualState.NotSetup or AvatarVisualState.Failed);
            _pendingContent.SetActive(state is AvatarVisualState.Pending);
        }
        
        private static Sprite ToSprite(Texture2D texture) =>
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        private void HandleTextureSet(Texture2D texture) => UpdateAvatarState();

        public void Unbind()
        {
            if (_model == null) return;
            _model.OnTextureSet -= HandleTextureSet;
            _model.OnTextureFailed -= UpdateAvatarState;
            _model = null;
        }

        private void OnDestroy() => Unbind();
    }
}