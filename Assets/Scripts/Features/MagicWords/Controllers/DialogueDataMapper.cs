using System.Collections.Generic;
using Core.Utils;
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
        public DialogueDataModel Map(IMagicWordsData data)
        {
            var avatarsByName = BuildAvatarDictionary(data);

            var lines = new List<DialogueEntryModel>(data.Dialogue.Count);
            foreach (var line in data.Dialogue)
            {
                avatarsByName.TryGetValue(line.Name, out var avatar);
                lines.Add(new DialogueEntryModel(line.Name, EmojiReplacer.ReplaceEmojiTokens(line.Text), avatar));
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
                    Debug.LogWarning($"Duplicate avatar data with same name: {name} {raw.Position} {raw.Url} skipping entry");
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