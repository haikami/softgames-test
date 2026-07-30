namespace Core.Interfaces
{
    /// <summary>
    /// Interface implemented by classes using a pool service
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnReturned();
    }
}