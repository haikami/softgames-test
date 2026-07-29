using System;
using System.Text;
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
            catch (UnityWebRequestException ex)
            {
                // ToUniTask throws for anything != Success, including a "successful"
                // HTTP error response (400/500 etc). ProtocolError means a real response
                // reached us — extract status + body instead of treating it as unreachable.
                if (ex.Result == UnityWebRequest.Result.ProtocolError)
                {
                    var bytes = string.IsNullOrEmpty(ex.Text)
                        ? Array.Empty<byte>()
                        : Encoding.UTF8.GetBytes(ex.Text);
                    return RawResponse.FromHttp(ex.ResponseCode, bytes);
                }

                // ConnectionError / DataProcessingError: no usable response at all.
                return RawResponse.TransportFailure(ex.Error);
            }

            return RawResponse.FromHttp((long)request.responseCode, request.downloadHandler.data);
        }
    }
}