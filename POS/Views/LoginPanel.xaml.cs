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

namespace POS.Views
{
    public partial class LoginPanel : Window
    {
        public bool isLoginValid;

        public LoginPanel()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (isLoginValid = AuthenticateUser(username, password))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Nieprawidłowe hasło lub login!");
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.Login == username && e.Password == password);
                if(user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}