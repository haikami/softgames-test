using Core.Configs;
using UnityEngine;

namespace Features.MagicWords.Configs
{
    [CreateAssetMenu(menuName = "Features/MagicWords/Config")]
    public class MagicWordsConfig : FeatureConfig
    {
        [Header("Endpoint")]
        [SerializeField] private string _endpointUrl;

        [Header("Timing")]
        [Tooltip("Time to wait after fetching dialogues so avatars can load")]
        [SerializeField] private float _avatarGraceSeconds = 1.5f;
        [SerializeField] private float _timeBetweenDialogueLines = 0.6f;

        [Header("Pooling")]
        [SerializeField] private int _initialDialogBubbles = 8;

        [Header("Local Testing Override")]
        [Tooltip("If set, skips the network call entirely and uses this data instead. Leave empty for real runs.")]
        [SerializeField] private MagicWordsLocalConfig _localSourceOverride;

        public string EndpointUrl => _endpointUrl;
        public float AvatarGraceSeconds => _avatarGraceSeconds;
        public float TimeBetweenDialogueLines => _timeBetweenDialogueLines;
        public int InitialDialogBubbles => _initialDialogBubbles;
        public MagicWordsLocalConfig LocalSourceOverride => _localSourceOverride;
    }
}