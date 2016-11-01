using System;
using System.Collections.Generic;
using System.Windows.Input;
using Core;
using Knuj.Interfaces;
using Knuj.Interfaces.Views;
using PropertyChanged;

namespace DemoApp.Controllers
{
    [ImplementPropertyChanged]
    public sealed class LoginController : ControllerBase
    {
        public ICommand DoLoginCommand { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        //private readonly LoginValidator _loginValidator;

        public LoginController() : this(null)
        {
        }

        public LoginController(object data) : base(data)
        {
            DoLoginCommand = new DependentRelayCommand(Login, Validate, this, () => Username, () => Password);
            //_loginValidator = new LoginValidator();
        }

        public void Login()
        {
            NavigationService.NavigateTo<FirstController>(new FirstControllerData { S = Password }, true);
        }

        public bool Validate()
        {
            //var result = _loginValidator.Validate(this);
            return true; // result.IsValid;
        }
    }

    public class app : IMobileApplication
    {
        public Dictionary<string, string> BeforeAutoLogoutData
        {
            get
           ;
            set
           ;
        }

        public IView BeforeAutoLogoutView
        {
            get
          ;
            set
           ;
        }

        public bool IsInBackground
        {
            get
          ;
            set
           ;
        }

        public IView LastActivity
        {
            get
           ;
            set
           ;
        }

        public object GetApplicationContext()
        {
            return null;
            // throw new NotImplementedException();
        }

        public void OnBackground(IView view)
        {
            //  throw new NotImplementedException();
        }

        public void OnForeground(IView view)
        {
            //throw new NotImplementedException();
        }

        public void RegisterContainers()
        {
            // throw new NotImplementedException();
        }
    }
}