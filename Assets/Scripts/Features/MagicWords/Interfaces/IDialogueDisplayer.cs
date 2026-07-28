using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    public interface IDialogueDisplayer
    {
        UniTask DisplayDialogue(IReadOnlyList<DialogueEntryModel> lines, Action<DialogueEntryModel> onLineReady, CancellationToken token);
    }
}