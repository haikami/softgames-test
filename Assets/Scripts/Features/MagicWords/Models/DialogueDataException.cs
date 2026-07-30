using System;

namespace Features.MagicWords.Models
{

    /// <summary>
    /// Used when fetching/validating dialogue data.
    /// </summary>
    public sealed class DialogueLoadingException : Exception
    {
        public DialogueLoadingException(string message) : base(message)
        {
        }
    }
}