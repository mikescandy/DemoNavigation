using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using CheeseBind;
using Core.Droid;
using DemoApp.Controllers;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [Activity(Label = "LoginActivity", MainLauncher = true)]
    public class LoginActivity : ActivityBase<LoginController>
    {
        [BindView(Resource.Id.username)]
        [Bind(Source = "Username", Target = "Text", BindingMode = BindingMode.TwoWay)]
        public EditText Username { get; set; }

        [BindView(Resource.Id.password)]
        [Bind(Source = "Password", Target = "Text", BindingMode = BindingMode.TwoWay)]
        public EditText Password { get; set; }

        [BindView(Resource.Id.login)]
        [BindCommand(Source = "DoLoginCommand")]
        //	[Bind(Source = "ButtonText", Target = "Text", BindingMode = BindingMode.OneWay)]
        public Button CameraButton { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState,     Resource.Layout.ActivityLogin);
            // Create your application here
        }
    }
}
