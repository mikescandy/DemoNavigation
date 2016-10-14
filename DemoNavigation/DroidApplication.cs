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
using Application = Android.App.Application;

namespace DemoNavigation
{
    [Application]
  public   class DroidApplication :Application
    {
        public static Core.Application app = Core.Application.Instance;
        /// <summary>
        /// Base constructor which must be implemented if it is to successfully inherit from the Application
        /// class.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownerShip"></param>
        public DroidApplication(IntPtr handle, JniHandleOwnership ownerShip)
            : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            var builder = new Autofac.ContainerBuilder();
            builder.RegisterType<MainActivity>().AsImplementedInterfaces();
            builder.RegisterType<FirstActivity>().AsImplementedInterfaces();
            builder.RegisterType<NavigationService>().AsImplementedInterfaces();
            builder.RegisterType<HomeController>();
            builder.RegisterType<FirstController>();
            app.Container = builder.Build();
        }
    }
}