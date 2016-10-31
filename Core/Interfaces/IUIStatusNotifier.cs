using Knuj.Interfaces.Views;

namespace Knuj.Interfaces
{
    public interface IUiStatusNotifier
    {
        bool OnForeground(IView view);
        bool OnBackground(IView view);
    }
}