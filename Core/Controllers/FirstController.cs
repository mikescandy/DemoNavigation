using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using PropertyChanged;

namespace Core
{
    [ImplementPropertyChanged]
    public class FirstController : ControllerBase
    {
        public string Text { get; set; }
        public RelayCommand DoSomethingCommand { get; set; }
        public async void DoSomething()
        {
            await CrossMedia.Current.Initialize();
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

            }

        }


        public FirstController() : this(null)
        {
        }

        public FirstController(FirstControllerData data) : base(data)
        {
            Text = data != null ? data.S ?? "blah" : "no data";
            DoSomethingCommand = new RelayCommand(DoSomething);
        }

        public override void ReverseInit(object data)
        {
            NavigationService.NavigateTo<SecondController>(false);
        }
    }

    public class FirstControllerData
    {
        public string S { get; set; }
    }
}
