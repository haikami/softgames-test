using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(menuName = "Features/Feature Definition")]
    public class FeatureDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private SceneField _scene;

        public string DisplayName => _displayName;
        public string SceneName => _scene.SceneName;
    }
}