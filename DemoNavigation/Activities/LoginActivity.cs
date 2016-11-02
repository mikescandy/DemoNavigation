Using Android.App;
using Android.OS;
using Core.Droid;
using DemoApp.Controllers;

namespace DemoNavigation
{
    [Activity(Label = "LoginActivity", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light")]
    public class LoginActivity : ActivityBase<LoginController>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState, Resource.Layout.ActivityLogin);
        }
    }
}