using FluentValidation;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Plugin.Media;
using PropertyChanged;

namespace Core
{
    [ImplementPropertyChanged]
    public sealed class LoginController : ControllerBase
    {
        public RelayCommand DoLoginCommand { get; set; }
        
        public string Username { get; set; }
		public string Password { get; set; }

		private readonly LoginValidator _loginValidator;

        public LoginController() : this(null)
        {
        }

        public LoginController(object data) : base(data)
        {
			DoLoginCommand = new RelayCommand(Login, IsValid);
			_loginValidator = new LoginValidator();
        }

        public void Login()
        {
            NavigationService.NavigateTo<FirstController>(new FirstControllerData { S = Password }, true);
        }

		public bool IsValid()
		{
			var result = _loginValidator.Validate(this);
			return result.IsValid;
		}
    }

	public class LoginValidator : AbstractValidator<LoginController>
	{
		public LoginValidator()
		{
			RuleFor(controller => controller.Username).NotEmpty().Must(BeAValidEmail);
			RuleFor(controller => controller.Password).NotEmpty();
		}

		private bool BeAValidEmail(string username)
		{
			return true;
		}
	}

}
