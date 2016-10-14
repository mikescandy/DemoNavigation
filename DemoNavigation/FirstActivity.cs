using Android.App;
using Android.Widget;
using Android.OS;
using Autofac;
using Core;

namespace DemoNavigation
{
    [Activity(Label = "FirstActivity", Icon = "@drawable/icon")]
    public class FirstActivity : ActivityBase<FirstController>, IFirstView
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }

        public FirstActivity()
        {
        }

        public FirstActivity(object parameters) : base(parameters)
        {
        }
    }
}

