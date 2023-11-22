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
    /// <summary>
    /// Logika interakcji dla klasy LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : Window
    {
        private readonly AppDbContext dbContext;
        public LoginPanel()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            bool isValidLogin = CheckLoginInDatabase(username, password);

            if (isValidLogin)
            {
                SalesPanel salesPanel = new SalesPanel();
                salesPanel.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nieprawidłowe hasło lub login!");
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }
        }

        private bool CheckLoginInDatabase(string username, string password)
        {
            var user = dbContext.Employees.FirstOrDefault(e => e.Login == username && e.Password == password);

            return user != null;
        }
    }
}
