using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    /// <summary>
    /// Map a list of dialogues with a list of avatars
    /// into a class that can be fed to different views. 
    /// </summary>
    public interface IDialogueDataMapper
    {
        DialogueDataModel Map(IMagicWordsData data);
    }
}