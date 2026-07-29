namespace Core.Networking
{
    public class NetworkRequestOptions
    {
        public int MaxRetries { get; }
        public float RetryDelaySeconds { get; }
        public int TimeoutSeconds { get; }

        public NetworkRequestOptions(int maxRetries = 2, float retryDelaySeconds = 1f, int timeoutSeconds = 5)
        {
            MaxRetries = maxRetries;
            RetryDelaySeconds = retryDelaySeconds;
            TimeoutSeconds = timeoutSeconds;
        }

        public static readonly NetworkRequestOptions Default = new();
    }
}