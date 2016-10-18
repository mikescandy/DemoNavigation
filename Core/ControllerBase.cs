using System.Runtime.InteropServices;
using Autofac;
using GalaSoft.MvvmLight.Messaging;

namespace Core
{
    public abstract class ControllerBase : IControllerBase
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
    }
}