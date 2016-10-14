using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Core
{
    public abstract class ControllerBase<T> : IControllerBase<T> where T : IView
    {
        public INavigationService NavigationService = Application.Instance.Container.Resolve<INavigationService>();

        public ControllerBase()
        {

        }

        public ControllerBase(object parameters)
        {

        }
    }
}
