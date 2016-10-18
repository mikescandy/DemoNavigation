using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;

namespace Core
{
    [ImplementPropertyChanged]
    public class HomeController : ControllerBase
    {
        public RelayCommand DoSomethingCommand { get; set; }
        public string EditText { get; set; }
        public string ButtonText { get; set; }

        public HomeController() : this(null)
        {
        }

        public HomeController(object data) : base(data)
        {
            DoSomethingCommand = new RelayCommand(DoSomething);
        }

        public void DoSomething()
        {
            NavigationService.NavigateTo<FirstController>(new FirstControllerData { S = EditText }, true);
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
