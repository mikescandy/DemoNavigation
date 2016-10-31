using System.ComponentModel;
using Knuj.Interfaces;

namespace Core
{
    public interface IControllerBase : IUiStatusNotifier, INotifyPropertyChanged
    {
        void ReverseInit(object data);
        string Title { get; set; }
        void Refresh();
    }
}