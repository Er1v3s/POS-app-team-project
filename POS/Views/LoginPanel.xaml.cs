using Microsoft.EntityFrameworkCore;
using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace POS.Views
{
    public partial class LoginPanel : Window
    {
        public bool isLoginValid;
        public bool isUserLoggedIn;
        public int employeeId;
        private readonly string uri;

        public LoginPanel(string uri = "")
        {
            this.uri = uri;
            InitializeComponent();
        }

        private void CloseLoginPanel(object sender, EventArgs e)
        {
            isUserLoggedIn = false;
            this.Close();
        }

        private void ValidateLoginEvent(object sender, EventArgs e)
        {
            isLoginValid = true;
            isUserLoggedIn = false;
            this.Close();
        }

        private void LogIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            employeeId = AuthenticateUserAndGetEmployeeId(username, password);

            if (employeeId != 0)
            {
                if (!isUserLoggedIn)
                {
                    StartFinishWork startFinishWork = new StartFinishWork(employeeId);
                    loginPanelWindow.Child = startFinishWork;
                    startFinishWork.StartWork.Click += ValidateLoginEvent;
                }
                else if(uri == "./StartFinishWork.xaml")
                {
                    StartFinishWork startFinishWork = new StartFinishWork(employeeId);
                    loginPanelWindow.Child = startFinishWork;
                    startFinishWork.StartWork.Click += CloseLoginPanel;
                    startFinishWork.FinishWork.Click += CloseLoginPanel;
                }
                else
                {
                    isLoginValid = true;
                    isUserLoggedIn = false;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowe hasło lub login!");
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }
        }

        private int AuthenticateUserAndGetEmployeeId(string username, string password)
        {
            using (var dbContext = new AppDbContext())
            {
                dbContext.SaveChanges();
                var user = dbContext.Employees.FirstOrDefault(e => e.Login == username && e.Password == password);
                if (user != null)
                {
                    if(user.Is_User_LoggedIn)
                    {
                        isUserLoggedIn = true;
                    }

                    return user.Employee_id;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}