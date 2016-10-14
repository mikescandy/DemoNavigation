using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Autofac;
using CheeseBind;
using Core;
using Application = Core.Application;

namespace DemoNavigation
{
    [Activity(Label = "DemoNavigation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ActivityBase<HomeController>, IHomeView
    {
        [OnClick(Resource.Id.HomeButton)]
        private void Button_OnClick(object sender, EventArgs eventArgs)
        {
            Controller.DoSomething();
        }

        public MainActivity()
        {
        }

        public MainActivity(object parameters) : base(parameters)
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}