namespace Core.Interfaces
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnReturned();
    }
}