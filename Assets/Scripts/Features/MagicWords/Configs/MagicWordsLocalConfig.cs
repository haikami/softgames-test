using System.Collections.Generic;
using Features.MagicWords.Interfaces;
using Features.MagicWords.Models;
using UnityEngine;

[CreateAssetMenu(menuName = "Features/MagicWords/Local Source Config")]
public class MagicWordsLocalConfig : ScriptableObject, IMagicWordsData
{
    [SerializeField] private List<DialogueLine> _dialogue;
    [SerializeField] private List<AvatarInfo> _avatars;

    public List<DialogueLine> Dialogue => _dialogue;
    public List<AvatarInfo> Avatars => _avatars;
}