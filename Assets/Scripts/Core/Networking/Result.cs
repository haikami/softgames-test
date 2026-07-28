namespace Core.Networking
{
    public readonly struct Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public NetworkError Error { get; }

        private Result(bool isSuccess, T value, NetworkError error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, default);
        public static Result<T> Failure(NetworkError error) => new(false, default, error);
    }
}