using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(menuName = "Emoji Config")]
    public class EmojiConfig : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string Token;
            public string EmojiUnicode;
        }

        [SerializeField] private List<Entry> _entries;
        public IReadOnlyDictionary<string, string> MapEntries() 
        {
            var dictionary = new Dictionary<string, string>(_entries.Count);

            foreach (var entry in _entries)
                dictionary.Add(entry.Token, entry.EmojiUnicode);

            return dictionary;
        }
    }
}