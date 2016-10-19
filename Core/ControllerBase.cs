using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using Core.Annotations;

namespace Core
{
    public abstract class ControllerBase : IControllerBase, INotifyPropertyChanged
    {
        public INavigationService NavigationService = Application.Instance.Container.Resolve<INavigationService>();

        protected ControllerBase() : this(null)
        {
        }

        protected ControllerBase(object data)
        {
        }

        public virtual void ReverseInit(object data)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}