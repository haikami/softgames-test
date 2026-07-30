using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.MagicWords.Models;

namespace Features.MagicWords.Interfaces
{
    /// <summary>
    /// Interface that receives a list of dialogues and
    /// calls onLineReady each time a certain line needs to be displayed.
    /// </summary>
    public interface IDialogueDisplayer
    {
        UniTask DisplayDialogue(IReadOnlyList<DialogueEntryModel> lines, Action<DialogueEntryModel> onLineReady, CancellationToken token);
    }
}