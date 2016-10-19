namespace Core
{
    public interface INavigationService
    {
        bool HasData();
        bool HasReturnData();
        object GetData();
        object GetReturnData();
        void NavigateTo<T>() where T : IControllerBase;
        void NavigateTo<T>(bool noHistory) where T : IControllerBase;
        void NavigateTo<T>(object data) where T : IControllerBase;
        void NavigateTo<T>(object data, bool noHistory) where T : IControllerBase;
        void Close();
        void Back();
        void Close(object data);
        void Back(object data);
    }
}