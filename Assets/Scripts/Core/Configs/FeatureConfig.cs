using UnityEngine;

namespace Core.Configs
{
    public class FeatureConfig : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private SceneField _scene;

        public string DisplayName => _displayName;
        public string SceneName => _scene.SceneName;
    }
}