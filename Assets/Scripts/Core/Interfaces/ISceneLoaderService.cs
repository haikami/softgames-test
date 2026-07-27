using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISceneLoaderService
    {
        UniTask LoadAdditive(string sceneName);
        UniTask UnloadAdditive(string sceneName);
        bool IsLoaded(string sceneName);
    }
}