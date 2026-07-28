using System.Collections.Generic;
using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    public interface IMagicWordsData
    {
        List<DialogueLine> Dialogue { get; }
        List<AvatarInfo> Avatars { get; }
    }
}