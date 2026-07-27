using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SceneField
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;
#endif
        [SerializeField] private string _sceneName;

        public string SceneName => _sceneName;
    }
}