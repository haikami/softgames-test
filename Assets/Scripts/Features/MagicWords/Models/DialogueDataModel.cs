using System.Collections.Generic;

namespace Features.MagicWords.Models
{
    /// <summary>
    /// Holds the list of dialogue lines parsed into models, along with a lookup dictionary for avatar models.
    /// </summary>
    public sealed class DialogueDataModel
    {
        public IReadOnlyList<DialogueEntryModel> Lines { get; }
        public IReadOnlyDictionary<string, AvatarModel> AvatarsByName { get; }

        public DialogueDataModel(IReadOnlyList<DialogueEntryModel> lines, IReadOnlyDictionary<string, AvatarModel> avatarsByName)
        {
            Lines = lines;
            AvatarsByName = avatarsByName;
        }
        
        public bool HasDialogues => Lines.Count > 0;
    }
}