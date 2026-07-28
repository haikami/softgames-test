namespace Core.Networking
{
    public readonly struct NetworkError
    {
        public NetworkErrorType Type { get; }
        public long HttpStatusCode { get; }
        public string Message { get; }

        private NetworkError(NetworkErrorType type, long httpStatusCode, string message)
        {
            Type = type;
            HttpStatusCode = httpStatusCode;
            Message = message;
        }

        public static NetworkError Cancelled() => new(NetworkErrorType.Cancelled, 0, "Request was cancelled.");
        public static NetworkError Unreachable(string reason) => new(NetworkErrorType.Unreachable, 0, reason);
        public static NetworkError Http(long statusCode, string message) => new(NetworkErrorType.Http, statusCode, message);
        public static NetworkError ParseFailure(string reason) => new(NetworkErrorType.ParseFailure, 0, reason);

        public override string ToString() => $"[{Type}] {Message} (status: {HttpStatusCode})";
    }
}