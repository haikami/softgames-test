using System;
using System.Collections.Generic;
using Features.MagicWords.Interfaces;
using Newtonsoft.Json;

namespace Features.MagicWords.Models
{
    /// <summary>
    /// Expected response from provided endpoint
    /// </summary>
    public class MagicWordsResponse : IMagicWordsData
    {
        [JsonProperty("dialogue")] public List<DialogueLine> Dialogue { get; set; }
        [JsonProperty("avatars")] public List<AvatarInfo> Avatars { get; set; }
    }

    [Serializable]
    public class DialogueLine
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("text")] public string Text;
    }

    [Serializable]
    public class AvatarInfo
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("url")] public string Url;
        [JsonProperty("position")] public string Position;
    }
}