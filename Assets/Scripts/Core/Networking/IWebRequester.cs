using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Networking
{
    /// <summary>
    /// Since we only need Get for the task, omitting put and post for the sake of simplicity
    /// </summary>
    public interface IWebRequester
    {
        UniTask<RawResponse> Get(string url, int timeoutSeconds, CancellationToken token);
    }
}