using System.Collections.Generic;
using Knuj.Interfaces.Views;

namespace Knuj.Interfaces
{
    public interface IMobileApplication
    {
        void RegisterContainers();
        object GetApplicationContext();
        void OnForeground(IView view);
        void OnBackground(IView view);
        IView BeforeAutoLogoutView { get; set; }
        Dictionary<string, string> BeforeAutoLogoutData { get; set; }
        bool IsInBackground { get; set; }
        IView LastActivity { get; set; }
    }
}