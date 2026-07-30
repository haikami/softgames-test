using System.Collections.Generic;
using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    /// <summary>
    /// Interface used so we can have both a server response and local config data
    /// as valid sources of information
    /// </summary>
    public interface IMagicWordsData
    {
        List<DialogueLine> Dialogue { get; }
        List<AvatarInfo> Avatars { get; }
    }
}