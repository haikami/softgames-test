using System.Collections.Generic;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Services
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly HashSet<string> _loadedScenes = new();

        public bool IsLoaded(string sceneName) => _loadedScenes.Contains(sceneName);

        public async UniTask LoadAdditive(string sceneName)
        {
            if (IsLoaded(sceneName)) return;
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Add(sceneName);
        }

        public async UniTask UnloadAdditive(string sceneName)
        {
            if (!IsLoaded(sceneName)) return;
            await SceneManager.UnloadSceneAsync(sceneName);
            _loadedScenes.Remove(sceneName);
        }
    }
}