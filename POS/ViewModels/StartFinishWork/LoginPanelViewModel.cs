using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.StartFinishWork
{
    public class LoginPanelViewModel : ViewModelBase
    {
        private readonly LoginService _loginService;

        private string login;
        private string password;

        public string Login
        {
            get => login;
            set => SetField(ref login, value);
        }

        public string Password
        {
            get => password;
            set => SetField(ref password, value);
        }

        public ICommand LoginCommand { get; }

        public LoginPanelViewModel(LoginService loginService)
        {
            _loginService = loginService;

            LoginCommand = new RelayCommandAsync(ExecuteLoginAsync);
        }

        private async Task ExecuteLoginAsync()
        {
            try
            {
                await _loginService.AuthenticateUserAsync(login, password);
            }
            catch (Exception)
            {
                MessageBox.Show("Nieprawidłowy login lub hasło!");
            }
        }
    }
}