using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Networking
{
    /// <summary>
    /// Provides networking functionality with support for retries, timeouts,
    /// request cancellation, JSON deserialization, and texture downloads. Active requests
    /// are tracked by owner, allowing related operations to be cancelled together.
    /// </summary>
    public class NetworkService : INetworkService
    {
        private readonly IWebRequester _requester;
        private readonly Dictionary<string, List<CancellationTokenSource>> _activeByOwner = new();

        public NetworkService(IWebRequester requester) => _requester = requester;

        public async UniTask<Result<T>> GetJson<T>(string url, string owner, NetworkRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= NetworkRequestOptions.Default;
            var raw = await ExecuteWithRetry(url, owner, options, cancellationToken);

            if (raw.WasCancelled) return Result<T>.Failure(NetworkError.Cancelled());
            if (!raw.TransportSucceeded) return Result<T>.Failure(NetworkError.Unreachable(raw.TransportError));
            if (!raw.IsHttpSuccess) return Result<T>.Failure(BuildHttpError(raw));

            try
            {
                var json = Encoding.UTF8.GetString(raw.Data);
                var value = JsonConvert.DeserializeObject<T>(json);
                return Result<T>.Success(value);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(NetworkError.ParseFailure(ex.Message));
            }
        }

        public async UniTask<Result<Texture2D>> GetTexture(string url, string owner, NetworkRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= NetworkRequestOptions.Default;
            var raw = await ExecuteWithRetry(url, owner, options, cancellationToken);

            if (raw.WasCancelled) return Result<Texture2D>.Failure(NetworkError.Cancelled());
            if (!raw.TransportSucceeded) return Result<Texture2D>.Failure(NetworkError.Unreachable(raw.TransportError));
            if (!raw.IsHttpSuccess) return Result<Texture2D>.Failure(BuildHttpError(raw));

            var texture = new Texture2D(2, 2);
            bool loaded = raw.Data is { Length: > 0 } && ImageConversion.LoadImage(texture, raw.Data, markNonReadable: false);

            if (!loaded)
            {
                UnityEngine.Object.Destroy(texture);
                return Result<Texture2D>.Failure(NetworkError.ParseFailure("Response was not an image."));
            }

            return Result<Texture2D>.Success(texture);
        }

        public void CancelAll(string owner)
        {
            if (!_activeByOwner.TryGetValue(owner, out var sources)) return;
            foreach (var cts in sources) cts.Cancel();
            sources.Clear();
        }

        private async UniTask<RawResponse> ExecuteWithRetry(string url, string owner, NetworkRequestOptions options, CancellationToken externalToken)
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            Track(owner, linked);

            try
            {
                RawResponse last = default;
                for (int attempt = 0; attempt <= options.MaxRetries; attempt++)
                {
                    last = await _requester.Get(url, options.TimeoutSeconds, linked.Token);

                    if (last.WasCancelled) return last;
                    if (last.IsHttpSuccess) return last;
                    if (!ShouldRetry(last, attempt, options)) return last;

                    await UniTask.Delay(TimeSpan.FromSeconds(options.RetryDelaySeconds), cancellationToken: linked.Token);
                }
                return last;
            }
            finally
            {
                Untrack(owner, linked);
            }
        }

        private static bool ShouldRetry(RawResponse response, int attempt, NetworkRequestOptions options)
        {
            if (attempt >= options.MaxRetries) return false;
            if (!response.TransportSucceeded) return true;   // no connection / DNS / timeout — worth retrying
            return response.StatusCode >= 500;                // server error — worth retrying; 4xx won't fix itself
        }

        private static NetworkError BuildHttpError(RawResponse raw)
        {
            var apiError = ApiErrorPayload.TryParse(raw.Data);
            var message = apiError?.Message ?? $"HTTP {raw.StatusCode}";
            return NetworkError.Http(raw.StatusCode, message);
        }

        private void Track(string owner, CancellationTokenSource cts)
        {
            if (!_activeByOwner.TryGetValue(owner, out var list))
            {
                list = new List<CancellationTokenSource>();
                _activeByOwner[owner] = list;
            }
            list.Add(cts);
        }

        private void Untrack(string owner, CancellationTokenSource cts)
        {
            if (_activeByOwner.TryGetValue(owner, out var list)) list.Remove(cts);
        }
    }
}