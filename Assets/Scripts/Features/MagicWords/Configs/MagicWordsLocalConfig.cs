using System.Collections.Generic;
using Features.MagicWords.Interfaces;
using Features.MagicWords.Models;
using UnityEngine;

/// <summary>
/// Used for creating fake dialogs
/// and assigning them avatars for testing.
/// </summary>
[CreateAssetMenu(menuName = "Features/MagicWords/Local Source Config")]
public class MagicWordsLocalConfig : ScriptableObject, IMagicWordsData
{
    [SerializeField] private List<DialogueLine> _dialogue;
    [SerializeField] private List<AvatarInfo> _avatars;
    [SerializeField] private float _fakeFetchDelay;

    public List<DialogueLine> Dialogue => _dialogue;
    public List<AvatarInfo> Avatars => _avatars;
    public float FakeFetchDelay => _fakeFetchDelay;
}