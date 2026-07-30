using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for a service that asynchronously loads and unloads scenes.
    /// </summary>
    public interface ISceneLoaderService
    {
        UniTask LoadAdditive(string sceneName);
        UniTask UnloadAdditive(string sceneName);
        bool IsLoaded(string sceneName);
    }
}