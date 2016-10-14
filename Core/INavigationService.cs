using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : IControllerBase;
        void NavigateTo<T>(object parameters) where T : IControllerBase;
    }
}
