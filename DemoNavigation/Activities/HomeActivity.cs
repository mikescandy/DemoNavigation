using Android.App;
using Android.Widget;
using Android.OS;
using CheeseBind;
using Core;
using Core.Droid;
using DemoApp.Controllers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [Activity(Label = "DemoNavigation", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
    public class HomeActivity : ActivityBase<HomeController>
    {
        [BindView(Resource.Id.imageView1)]
        [BindImage(Source = "Image")]
        public ImageView Image { get; set; }

        protected Binding<string, string> EditTextBinding { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            OnCreate(bundle, Resource.Layout.Main);
        }
    }
}
