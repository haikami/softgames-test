using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Networking;
using Cysharp.Threading.Tasks;

namespace Core.Tests.EditMode.Networking
{
    /// <summary>
    /// Plays back a sequence of responses (one per call), or can be told to
    /// "hang" on the next call until manually completed / cancelled — used to test
    /// retry logic and mid-flight cancellation without any real networking.
    /// </summary>
    public class FakeWebRequester : IWebRequester
    {
        public int CallCount { get; private set; }
        public IReadOnlyList<string> RequestedUrls => _requestedUrls;

        private readonly List<string> _requestedUrls = new();
        private readonly Queue<RawResponse> _scriptedResponses;
        private TaskCompletionSource<RawResponse> _pendingCompletion;

        public FakeWebRequester(params RawResponse[] responses)
        {
            _scriptedResponses = new Queue<RawResponse>(responses);
        }
        
        public void HangNextCall() => _pendingCompletion = new TaskCompletionSource<RawResponse>();

        public async UniTask<RawResponse> Get(string url, int timeoutSeconds, CancellationToken token)
        {
            CallCount++;
            _requestedUrls.Add(url);

            if (_pendingCompletion != null)
            {
                var tcs = _pendingCompletion;
                _pendingCompletion = null;

                await using var registration = token.Register(() => tcs.TrySetCanceled());
                try
                {
                    return await tcs.Task;
                }
                catch (TaskCanceledException)
                {
                    return RawResponse.Cancelled();
                }
                catch (OperationCanceledException)
                {
                    return RawResponse.Cancelled();
                }
            }

            if (token.IsCancellationRequested) return RawResponse.Cancelled();

            if (_scriptedResponses.Count == 0)
                throw new InvalidOperationException(
                    $"{nameof(FakeWebRequester)} ran out of scripted responses on call #{CallCount}. " +
                    "Script one more RawResponse for this test.");

            return _scriptedResponses.Dequeue();
        }
    }
}
