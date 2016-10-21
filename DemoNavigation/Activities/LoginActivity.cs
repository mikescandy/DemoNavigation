using System;
using Android.App;
using Android.OS;
using Android.Views;
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
        [Bind(Source = "Password",
              Target = "Visibility",
              BindingMode = BindingMode.OneWay,
            SourceToTargetConverter = "StringToVisibilityConverter",
            
            Converters = typeof(Converters))]
        public EditText Password { get; set; }

        [BindView(Resource.Id.login)]
     //   [BindCommand(Source = "DoLoginCommand")]
        //	[Bind(Source = "ButtonText", Target = "Text", BindingMode = BindingMode.OneWay)]
        public Button CameraButton { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState, Resource.Layout.ActivityLogin);
            //Username.SetBinding<string, string>(() => Username.Text, () => Controller.Username, BindingMode.TwoWay);
            //Controller.SetBinding<string, string>("Username", Username, "Text", BindingMode.OneWay);

         //   Controller.SetBinding<string, ViewStates>("Password", Password, "Visibility", BindingMode.OneWay).ConvertSourceToTarget(Converters.StringToVisibilityConverter);
           
            // Create your application here
            // CameraButton.Click += (sender, args) => { Password.Visibility = ViewStates.Gone; };


        }
    }

    public static class Converters
    {
        public static ViewStates StringToVisibility(string value)
        {
            return value == "goaway" ? ViewStates.Gone : ViewStates.Visible;
        } 

        public static Func<string, ViewStates> StringToVisibilityConverter
        {
            get
            {
                return s =>
                {
                    if (s == "goaway")
                    {
                        return ViewStates.Gone;
                    }
                    return ViewStates.Visible;
                };
            }
        }

        public static Func<ViewStates, string> VisibilityToStringConverter
        {
            get
            {
                return s => "";
            }
        }
    }
}
