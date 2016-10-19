using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Core
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
