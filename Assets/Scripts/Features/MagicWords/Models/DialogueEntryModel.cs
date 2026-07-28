namespace Features.MagicWords.Models
{
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