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
using Core.Droid;
using DemoApp.Controllers;
using Plugin.CurrentActivity;
using Application = Android.App.Application;

namespace DemoNavigation
{
    [Application]
  public class DroidApplication :Application, Application.IActivityLifecycleCallbacks
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
            builder.RegisterType<NavigationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<HomeController>();
            builder.RegisterType<FirstController>();
            builder.RegisterType<SecondController>();
            builder.RegisterType<LoginController>();
            app.Container = builder.Build();
            RegisterActivityLifecycleCallbacks(this);
         }

       

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}