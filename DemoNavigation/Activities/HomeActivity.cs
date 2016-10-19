using Android.App;
using Android.Widget;
using Android.OS;
using CheeseBind;
using Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [Activity(Label = "DemoNavigation", MainLauncher = false, Icon = "@drawable/icon")]
    public class HomeActivity : ActivityBase<HomeController>
    {
        [BindView(Resource.Id.HomeButton)]
        [BindCommand(Source = "DoSomethingCommand", Target = "Click")]
        [Bind(Source = "ButtonText", Target = "Text", BindingMode = BindingMode.OneWay)]
        public Button Button { get; set; }

		[BindView(Resource.Id.CameraButton)]
		[BindCommand(Source = "DoTakePictureCommand", Target = "Click")]
		[Bind(Source = "ButtonText", Target = "Text", BindingMode = BindingMode.OneWay)]
		public Button CameraButton { get; set; }


		[BindView(Resource.Id.editText1)]
        [Bind(Source = "EditText", Target = "Text", BindingMode = BindingMode.TwoWay)]
        public EditText EditText { get; set; }

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
