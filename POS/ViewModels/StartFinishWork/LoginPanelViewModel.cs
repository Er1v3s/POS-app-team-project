using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POS.Services.Login;
using POS.Utilities;
using POS.Utilities.RelayCommands;

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
            set
            {
                SetField(ref login, value);
                OnPropertyChanged(nameof(Login));
            }
        }

        public string Password
        {
            get => password;
            set
            {
                SetField(ref password, value);
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginPanelViewModel(LoginService loginService)
        {
            _loginService = loginService;

            LoginCommand = new RelayCommandAsync(ExecuteLogin);
        }

        private async Task ExecuteLogin()
        {
            try
            {
                await _loginService.AuthenticateUser(login, password);
            }
            catch (Exception)
            {
                MessageBox.Show("Nieprawidłowy login lub hasło!");
            }
        }
    }
}
