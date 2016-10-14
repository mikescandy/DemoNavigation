using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class HomeController : ControllerBase<IHomeView>
    {
        public HomeController()
        {

        }

        public HomeController(object parameters) : base(parameters)
        {

        }

        public void DoSomething()
        {
            NavigationService.NavigateTo<FirstController>("test");
        }
    }
}
