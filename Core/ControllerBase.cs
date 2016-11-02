using System.ComponentModel;
using System.Runtime.CompilerServices;
using Knuj.Interfaces;
using Knuj.Interfaces.Views;

namespace Core
{
    public abstract class ControllerBase : IControllerBase
    {
        public string Title { get; set; }
        public INavigationService NavigationService = Container.Instance.Resolve<INavigationService>();
       //public IMobileApplication MobileApplication = Container.Instance.Resolve<IMobileApplication>();
        //public ICoreCache Cache = Container.Instance.Resolve<ICoreCache>();

        protected ControllerBase() : this(null)
        {
        }

        protected ControllerBase(object data)
        {
        }

        public virtual void ReverseInit(object data)
        {
        }

        public virtual void Refresh()
        {
        }

        public bool OnForeground(IView view)
        {
            Container.Instance.Resolve<IMobileApplication>().OnForeground(view);
            return true;
        }

        public bool OnBackground(IView view)
        {
            Container.Instance.Resolve<IMobileApplication>().OnBackground(view);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}