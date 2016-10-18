using Android.App;
using Android.OS;
using Android.Widget;
using CheeseBind;
using Core;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [Activity(Label = "FirstActivity", Icon = "@drawable/icon")]
    public class FirstActivity : ActivityBase<FirstController>
    {
        [BindView(Resource.Id.HomeButton)]
        public Button Button { get; set; }

        [BindView(Resource.Id.textView1)]
        public TextView TextView { get; set; }
        protected Binding<string, string> TextViewBinding { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            OnCreate(bundle, Resource.Layout.First);
            // Set our view from the "main" layout resource
            Button.SetCommand("Click", Controller.DoSomethingCommand);
            TextViewBinding = this.SetBinding(() => Controller.Text, () => TextView.Text);
        }
    }
}