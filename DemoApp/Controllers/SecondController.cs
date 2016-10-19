using Core;
using GalaSoft.MvvmLight.Command;

namespace DemoApp.Controllers
{
    public class SecondController : ControllerBase
    {
        public RelayCommand DoSomethingCommand { get; set; }

        public void DoSomething()
        {
            NavigationService.Close(new HomeResult {Message="Ciao"});
        }

        public SecondController()
        {
            DoSomethingCommand = new RelayCommand(DoSomething);
        }

        public SecondController(object data) : base(data)
        {
        }
    }
}