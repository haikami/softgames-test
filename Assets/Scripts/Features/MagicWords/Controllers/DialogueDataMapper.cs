using System.Collections.Generic;
using System.Text.RegularExpressions;
using Features.MagicWords.Enums;
using Features.MagicWords.Interfaces;
using Features.MagicWords.Models;
using UnityEngine;

namespace Features.MagicWords.Controllers
{
    /// <summary>
    /// Creates avatar models from avatar info.
    /// Maps dialogue lines with corresponding avatar model.
    /// </summary>
    public class DialogueDataMapper : IDialogueDataMapper
    {
        private static readonly Regex TokenPattern = new(@"\{(\w+)\}", RegexOptions.Compiled);
        
        private readonly IReadOnlyDictionary<string, string> _map;

        private string ReplaceEmojis(string rawText)
        {
            if (string.IsNullOrEmpty(rawText)) return rawText;
            
            var result = TokenPattern.Replace(rawText, match =>
                _map.TryGetValue(match.Groups[1].Value, out var emoji) ? emoji : match.Value);

            
            return result;
        }
        
        public DialogueDataMapper(IReadOnlyDictionary<string, string> emojiMap) => _map = emojiMap;

        public DialogueDataModel Map(IMagicWordsData data)
        {
            var avatarsByName = BuildAvatarDictionary(data);

            var lines = new List<DialogueEntryModel>(data.Dialogue.Count);
            foreach (var line in data.Dialogue)
            {
                avatarsByName.TryGetValue(line.Name, out var avatar);
                lines.Add(new DialogueEntryModel(line.Name, ReplaceEmojis(line.Text), avatar));
            }

            return new DialogueDataModel(lines, avatarsByName);
        }

        /// <summary>
        /// Skips avatars with the same name, holding info of first found in list
        /// Skips avatars without any dialogue line
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Dictionary of available avatars with their models</returns>
        private static Dictionary<string, AvatarModel> BuildAvatarDictionary(IMagicWordsData data)
        {
            var avatars = data.Avatars;
            var dialogue = data.Dialogue;
            var map = new Dictionary<string, AvatarModel>();
            foreach (var raw in avatars)
            {
                var name = raw.Name;
                //if avatar doesnt appear in any dialog skip fetching it.
                if (dialogue.Find(line => line.Name == name) == null)
                {
                    Debug.LogWarning($"Skipping avatar entry {name}, no dialogue lines found");
                    continue;
                }
                //if it already exists, then keep the info of the one already in the dictionary
                if (!map.TryAdd(name, new AvatarModel(name, raw.Url, ParsePosition(raw.Position))))
                {
                    Debug.LogError($"Duplicate avatar data with same name: {name} {raw.Position} {raw.Url} skipping entry");
                }
            }
            
            return map;
        }

        private static AvatarPosition ParsePosition(string raw) => raw?.Trim().ToLowerInvariant() switch
        {
            "right" => AvatarPosition.Right,
            _ => AvatarPosition.Left // everything else
        };
    }
}