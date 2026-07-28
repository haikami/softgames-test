namespace Core.Interfaces
{
    public interface ILoadingScreen
    {
        //Caller could be used in case there are several objects needing to load stuff at the same time.
        //So screen won't go unless the last caller hides it.
        //Omitting that logic for the sake of simplicity in this test.
        void Show(object caller = null);
        void Hide(object caller = null);
    }
}