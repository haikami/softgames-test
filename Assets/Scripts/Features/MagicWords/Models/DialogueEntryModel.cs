namespace Features.MagicWords.Models
{
    /// <summary>
    /// Holds info of speaker name, formatted text and avatar model
    /// </summary>
    public class DialogueEntryModel
    {
        public string SpeakerName { get; }
        public string FormattedText { get; }
        public AvatarModel Avatar { get; } // null if no avatar entry matched this speaker

        public DialogueEntryModel(string speakerName, string formattedText, AvatarModel avatar)
        {
            SpeakerName = speakerName;
            FormattedText = formattedText;
            Avatar = avatar;
        }
    }
}