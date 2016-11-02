using Android.App;
using Android.OS;
using Core.Droid;
using DemoApp.Controllers;

namespace DemoNavigation
{
    [Activity(Label = "DemoNavigation", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
    public class HomeActivity : ActivityBase<HomeController>
    {
        protected override void OnCreate(Bundle bundle)
        {
            OnCreate(bundle, Resource.Layout.Main);
        }
    }
}