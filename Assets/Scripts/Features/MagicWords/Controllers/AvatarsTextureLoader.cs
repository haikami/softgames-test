using System;
using System.Collections.Generic;
using Core.Networking;
using Cysharp.Threading.Tasks;
using Features.MagicWords.Enums;
using Features.MagicWords.Models;

namespace Features.MagicWords.Controllers
{
    /// <summary>
    /// Starts loading all avatars asynchronously
    /// Updates avatar models when ready
    /// </summary>
    public class AvatarsTextureLoader
    {
        private readonly INetworkService _network;
        private readonly string _owner;

        public AvatarsTextureLoader(INetworkService network, string owner)
        {
            _network = network;
            _owner = owner;
        }

        private List<UniTask> GetListOfAvatarLoaders(IReadOnlyDictionary<string, AvatarModel> avatarsByName)
        {
            var tasks = new List<UniTask>();

            foreach (var avatar in avatarsByName.Values)
            {
                if (avatar.State != AvatarVisualState.Pending)
                    continue;

                tasks.Add(LoadOne(avatar));
            }

            return tasks;
        }
        
        public async UniTask LoadAllWithGrace(IReadOnlyDictionary<string, AvatarModel> avatarsByName, float graceSeconds)
        {
            var tasks = GetListOfAvatarLoaders(avatarsByName);

            if (tasks.Count == 0)
                return;

            var loadAll = UniTask.WhenAll(tasks);
            await UniTask.WhenAny(loadAll, UniTask.Delay(TimeSpan.FromSeconds(graceSeconds)));
        }

        private async UniTask LoadOne(AvatarModel avatar)
        {
            var result = await _network.GetTexture(avatar.ImageUrl, _owner);
            if (result.IsSuccess) avatar.SetTexture(result.Value);
            else avatar.SetFailed();
        }

        public void CancelAll() => _network.CancelAll(_owner);
    }
}