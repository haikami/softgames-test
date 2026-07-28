using Features.MagicWords.Enums;
using Features.MagicWords.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MagicWords.UI
{
    public class AvatarView : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _pendingSprite;

        private AvatarModel _model;

        public void Bind(AvatarModel model)
        {
            Unbind();
            _model = model;

            if (_model == null)
            {
                _avatarImage.sprite = _defaultSprite; // NotSetup: no avatar for this speaker at all
                return;
            }

            _model.OnTextureSet += HandleTextureSet;
            _model.OnTextureFailed += HandleTextureFailed;
            Render(_model.State, _model.Texture);
        }

        private void Render(AvatarVisualState state, Texture2D texture)
        {
            _avatarImage.sprite = state switch
            {
                AvatarVisualState.Ready => ToSprite(texture),
                AvatarVisualState.Pending => _pendingSprite,
                _ => _defaultSprite, // NotSetup and Failed share the same fallback visual
            };
        }

        private static Sprite ToSprite(Texture2D texture) =>
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        private void HandleTextureSet(Texture2D texture) => _avatarImage.sprite = ToSprite(texture);
        private void HandleTextureFailed() => _avatarImage.sprite = _defaultSprite;

        public void Unbind()
        {
            if (_model == null) return;
            _model.OnTextureSet -= HandleTextureSet;
            _model.OnTextureFailed -= HandleTextureFailed;
            _model = null;
        }

        private void OnDestroy() => Unbind();
    }
}