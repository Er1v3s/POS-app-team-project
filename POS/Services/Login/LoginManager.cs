using System;
using DataAccess.Models;
using POS.Views.Windows;

namespace POS.Services.Login
{
    public class LoginManager
    {
        public static LoginManager Instance { get; } = new LoginManager();
        public Employee? Employee { get; private set; }
        public bool IsAnySessionActive { get; set; }
        public bool IsAuthenticationOnlyRequired { get; set; }
        public bool SuccessfullyLoggedIn { get; set; }

        public void LogIn(Employee employee)
        {
            SuccessfullyLoggedIn = true;
            Employee = employee;
        }

        public void LogOut()
        {
            Employee = null;
        }

        public string GetLoggedInUserFullName()
        {
            if(Employee != null)
                return Employee.FirstName + " " + Employee.LastName;

            return String.Empty;
        }

        public static void OpenLoginWindow()
        {
            var loginView = new LoginPanelWindow();
            loginView.ShowDialog();
        }
    }
}