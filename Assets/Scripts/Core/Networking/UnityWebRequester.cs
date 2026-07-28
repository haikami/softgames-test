using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Core.Networking
{
    public class UnityWebRequester : IWebRequester
    {
        public async UniTask<RawResponse> Get(string url, int timeoutSeconds, CancellationToken token)
        {
            using var request = UnityWebRequest.Get(url);
            request.timeout = timeoutSeconds;

            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return RawResponse.Cancelled();
            }

            // ProtocolError still means we got a real HTTP response (e.g. 400/500) —
            // that's a valid "transport succeeded, application-level failure" case,
            // distinct from ConnectionError (no response reached us at all).
            var gotResponse = request.result is UnityWebRequest.Result.Success
                or UnityWebRequest.Result.ProtocolError;

            return gotResponse ? RawResponse.FromHttp((long)request.responseCode, request.downloadHandler.data)
            : RawResponse.TransportFailure(request.error);
        }
    }
}