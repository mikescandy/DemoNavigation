using Android.App;
using Android.OS;
using Android.Widget;
using CheeseBind;
using Core;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [Activity(Label = "SecondActivity", Icon = "@drawable/icon")]
    public class SecondActivity : ActivityBase<SecondController>
    {
        [BindView(Resource.Id.HomeButton)]
        public Button Button { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            OnCreate(bundle, Resource.Layout.Second);
            // Set our view from the "main" layout resource
            Button.SetCommand("Click", Controller.DoSomethingCommand);
        }
    }
}