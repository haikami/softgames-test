using System;
using Features.MagicWords.Enums;
using UnityEngine;

namespace Features.MagicWords.Models
{
    /// <summary>
    /// Model of an avatar that holds information of the current avatar state, its texture, and events
    /// that are triggered to notify when its texture is set/failed. 
    /// </summary>
    public class AvatarModel
    {
        public string Name { get; }
        public string ImageUrl { get; }
        public AvatarPosition Position { get; }
        public AvatarVisualState State { get; private set; }
        public Texture2D Texture { get; private set; }

        public event Action<Texture2D> OnTextureSet;
        public event Action OnTextureFailed;

        public AvatarModel(string name, string imageUrl, AvatarPosition position)
        {
            Name = name;
            ImageUrl = imageUrl;
            Position = position;
            State = string.IsNullOrEmpty(imageUrl) ? AvatarVisualState.NotSetup : AvatarVisualState.Pending;
        }

        public void SetTexture(Texture2D texture)
        {
            if (State != AvatarVisualState.Pending) return; // already resolved (or nothing to resolve) — ignore late/duplicate calls
            Texture = texture;
            State = AvatarVisualState.Ready;
            OnTextureSet?.Invoke(texture);
        }

        public void SetFailed()
        {
            if (State != AvatarVisualState.Pending) return;
            State = AvatarVisualState.Failed;
            OnTextureFailed?.Invoke();
        }
    }
}