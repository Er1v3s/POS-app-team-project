using DataAccess.Models;
using POS.Views.Windows;

namespace POS.Services.Login
{
    public class LoginManager
    {
        public static LoginManager Instance { get; } = new LoginManager();
        public Employees? Employee { get; private set; }
        public bool IsAnySessionActive { get; set; }
        public bool IsAuthenticationOnlyRequired { get; set; }
        public bool SuccessfullyLoggedIn { get; set; }

        public void LogIn(Employees employee)
        {
            SuccessfullyLoggedIn = true;
            Employee = employee;
        }

        public void LogOut()
        {
            Employee = null;
        }

        public static void OpenLoginWindow()
        {
            var loginView = new LoginPanelWindow();
            loginView.ShowDialog();
        }
    }
}