namespace Core.Networking
{
    public class NetworkRequestOptions
    {
        public int MaxRetries { get; set; } = 2;
        public float RetryDelaySeconds { get; set; } = 1f;
        public int TimeoutSeconds { get; set; } = 10;

        public static NetworkRequestOptions Default => new();
    }
}