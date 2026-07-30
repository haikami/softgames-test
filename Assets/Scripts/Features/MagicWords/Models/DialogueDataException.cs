using System;

namespace Features.MagicWords.Models
{
    public sealed class DialogueLoadingException : Exception
    {
        public DialogueLoadingException(string message) : base(message)
        {
        }
    }
}