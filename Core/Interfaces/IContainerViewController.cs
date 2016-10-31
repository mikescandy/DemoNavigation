using System;
using Core;

namespace Knuj.Interfaces.Views
{
    public interface IContainerViewController : IView
    {
        int GetContainerViewId();
        void SetTitle(string title);
        void SetSelectedFragment(IBackHandlerFragment backHandledFragment);
        void OnBackPressed();
        void ShowController(Type type);
        void ShowController();
    }
}