using System.Text.RegularExpressions;

namespace Core.Utils
{
    public static class EmojiReplacer
    {
        private static readonly Regex TokenPattern = new(@"\{(\w+)\}", RegexOptions.Compiled);
        
        public static string ReplaceEmojiTokens(string rawText)
        {
            return string.IsNullOrEmpty(rawText) ? 
                rawText : 
                TokenPattern.Replace(rawText, match => $"<sprite name=\"{match.Groups[1].Value}\">");
        }
    }
}