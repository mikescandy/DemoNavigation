using System.Windows.Input;
using Core;
using PropertyChanged;

namespace DemoApp.Controllers
{
    [ImplementPropertyChanged]
    public sealed class LoginController : ControllerBase
    {
        public ICommand DoLoginCommand { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        private readonly LoginValidator _loginValidator;

        public LoginController() : this(null)
        {
        }

        public LoginController(object data) : base(data)
        {
            DoLoginCommand = new DependentRelayCommand(() => {/* do nothing*/}, Validate, this, () => Username, () => Password);
            _loginValidator = new LoginValidator();
        }

        public void Login()
        {
            NavigationService.NavigateTo<FirstController>(new FirstControllerData { S = Password }, true);
        }

        public bool Validate()
        {
            var result = _loginValidator.Validate(this);
            return result.IsValid;
        }
    }
}