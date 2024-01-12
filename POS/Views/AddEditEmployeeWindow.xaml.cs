using POS.Models;
using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEditEmployeeWindow : Window
    {

        public AddEditEmployeeWindow()
        {
            InitializeComponent();

        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddNewEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string jobTitle = (txtJobTitle.SelectedItem as ComboBoxItem)?.Content.ToString();
            string email = txtEmail.Text;
            int phoneNumber = int.Parse(txtPhoneNumber.Text);
            string address = txtAdress.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Text;

            using (var dbContext = new AppDbContext())
            {
                Employees newEmployee = new Employees
                {
                    First_name = firstName,
                    Last_name = lastName,
                    Job_title = jobTitle,
                    Email = email,
                    Phone_number = phoneNumber,
                    Address = address,
                    Login = login,
                    Password = password,
                    Is_User_LoggedIn = false
                };

                dbContext.Employees.Add(newEmployee);
                dbContext.SaveChanges();
            }

            this.Close();
        }
    }
}
