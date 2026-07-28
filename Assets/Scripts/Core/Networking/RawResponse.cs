namespace Core.Networking
{
    public readonly struct RawResponse
    {
        public bool WasCancelled { get; }
        public bool TransportSucceeded { get; }
        public long StatusCode { get; }
        public byte[] Data { get; }
        public string TransportError { get; }

        private RawResponse(bool wasCancelled, bool transportSucceeded, long statusCode, byte[] data, string transportError)
        {
            WasCancelled = wasCancelled;
            TransportSucceeded = transportSucceeded;
            StatusCode = statusCode;
            Data = data;
            TransportError = transportError;
        }

        public bool IsHttpSuccess => TransportSucceeded && StatusCode is >= 200 and < 300;

        public static RawResponse Cancelled() => new(true, false, 0, null, null);
        public static RawResponse TransportFailure(string error) => new(false, false, 0, null, error);
        public static RawResponse FromHttp(long statusCode, byte[] data) => new(false, true, statusCode, data, null);
    }
}