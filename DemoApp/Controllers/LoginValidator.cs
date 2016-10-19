using FluentValidation;

namespace DemoApp.Controllers
{

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