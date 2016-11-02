using System;
using Knuj.Interfaces;
using Knuj.Interfaces.Views;

namespace Core
{
    public interface INavigationService
    {
        bool HasData();
        bool HasReturnData();
        bool HasSubView();
        object GetData();
        object GetReturnData();
        Type GetSubView();
        void NavigateTo<T>() where T : IControllerBase;
        void NavigateTo<T>(bool noHistory) where T : IControllerBase;
        void NavigateTo<T>(object data) where T : IControllerBase;
        void NavigateTo<T>(object data, bool noHistory) where T : IControllerBase;
        void NavigateToRoot<T>() where T : IControllerBase;
        void NavigateToRoot<T>(object data) where T : IControllerBase;
        void NavigateToSubView<TContainer, TTarget>() where TContainer : IContainerController where TTarget : IControllerBase;
        void Close();
        void Back();
        void Close(object data);
        void Back(object data);
        Type GetViewForController<T>() where T : IControllerBase;
        Type GetViewForController(Type type);
    }
}