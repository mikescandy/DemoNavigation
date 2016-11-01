using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Core.Droid;
using DemoApp.Controllers;
using GalaSoft.MvvmLight.Helpers;

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

    public static class Converters
    {
        public static Func<string, ViewStates> StringToVisibilityConverter
        {
            get
            {
                return s => s == "goaway" ? ViewStates.Gone : ViewStates.Visible;
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
