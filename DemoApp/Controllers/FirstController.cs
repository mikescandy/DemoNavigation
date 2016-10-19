using Core;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;

namespace DemoApp.Controllers
{
    [ImplementPropertyChanged]
    public class FirstController : ControllerBase
    {
        public string Text { get; set; }
        public RelayCommand DoSomethingCommand { get; set; }
        public void DoSomething()
        {
            NavigationService.NavigateTo<SecondController>(false);
        }


        public FirstController() : this(null)
        {
        }

        public FirstController(FirstControllerData data) : base(data)
        {
            Text = data != null ? data.S??"blah" : "no data";
            DoSomethingCommand = new RelayCommand(DoSomething);
        }
    }

    public class FirstControllerData
    {
        public string S { get; set; }
    }
}
