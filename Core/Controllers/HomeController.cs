using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Plugin.Media;
using PropertyChanged;

namespace Core
{
    [ImplementPropertyChanged]
    public class HomeController : ControllerBase
    {
        public RelayCommand DoSomethingCommand { get; set; }
        public RelayCommand DoTakePictureCommand { get; set; }
        public string EditText { get; set; }
        public string ButtonText { get; set; }
        public byte[] Image { get; set; }
        public HomeController() : this(null)
        {
        }

        public HomeController(object data) : base(data)
        {
            DoSomethingCommand = new RelayCommand(DoSomething);
            DoTakePictureCommand = new RelayCommand(DoTakePicture);
        }

        public void DoSomething()
        {
            NavigationService.NavigateTo<FirstController>(new FirstControllerData { S = EditText }, true);
        }

        public async void DoTakePicture()
        {
            await CrossMedia.Current.Initialize();
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                var stream = file.GetStream();
                await stream.ReadAsync(Image, 0, (int)stream.Length);
            }

        }

        public override void ReverseInit(object data)
        {
            var result = data as HomeResult;
            EditText = result?.Message ?? "No data";
        }
    }

    public class HomeResult
    {
        public string Message { get; set; }
    }
}
