using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using Core;

namespace DemoNavigation
{
    public abstract class ActivityBase<T> : Activity where T : IControllerBase
    {
        protected virtual T Controller { get; private set; }

        protected ActivityBase() : this(null)
        {

        }

        protected ActivityBase(object parameters)
        {
            Controller = ResolveController(parameters);
        }

        private T ResolveController(object parameters)
        {
            if (parameters != null)
            {
                return Core.Application.Instance.Container.Resolve<T>(new NamedParameter("parameters", parameters));
            }
            else
            {
                return Core.Application.Instance.Container.Resolve<T>();
            }

        }
    }
}