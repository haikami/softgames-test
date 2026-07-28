using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Networking
{
    public interface INetworkService
    {
        UniTask<Result<T>> GetJson<T>(string url, string owner, NetworkRequestOptions options = null, CancellationToken cancellationToken = default);
        UniTask<Result<Texture2D>> GetTexture(string url, string owner, NetworkRequestOptions options = null, CancellationToken cancellationToken = default);
        void CancelAll(string owner);
    }
}