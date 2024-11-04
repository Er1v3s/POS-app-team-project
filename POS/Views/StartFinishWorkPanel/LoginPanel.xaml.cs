using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;

namespace POS.Views.StartFinishWorkPanel
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

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
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
                    startFinishWork.StartWork.Click += CloseWindow_ButtonClick;
                    startFinishWork.FinishWork.Click += CloseWindow_ButtonClick;
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
                    if(user.IsUserLoggedIn)
                    {
                        isUserLoggedIn = true;
                    }

                    return user.EmployeeId;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}