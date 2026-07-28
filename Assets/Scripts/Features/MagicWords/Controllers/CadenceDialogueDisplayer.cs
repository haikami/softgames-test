using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.MagicWords.Interfaces;
using Features.MagicWords.Models;

namespace Features.MagicWords.Controllers
{
    public class CadenceDialogueDisplayer : IDialogueDisplayer
    {
        private readonly float _delayBetweenLines;

        public CadenceDialogueDisplayer(float delayBetweenLines) => _delayBetweenLines = delayBetweenLines;

        public async UniTask DisplayDialogue(IReadOnlyList<DialogueEntryModel> lines, Action<DialogueEntryModel> onLineReady, CancellationToken token)
        {
            foreach (var line in lines)
            {
                onLineReady(line);
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenLines), cancellationToken: token);
            }
        }
    }
}