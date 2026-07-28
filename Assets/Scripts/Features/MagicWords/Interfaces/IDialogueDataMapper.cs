using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    public interface IDialogueDataMapper
    {
        DialogueDataModel Map(IMagicWordsData data);
    }
}