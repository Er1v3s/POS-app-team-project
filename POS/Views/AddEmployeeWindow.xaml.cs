using POS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {

        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddNewEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            TryAddEmployee();
            this.Close();
        }

        private int ParsePhoneNumber(string txtPhoneNumber)
        {
            int intPhoneNumber;

            if(int.TryParse(txtPhoneNumber, out intPhoneNumber))
            {
                return intPhoneNumber;
            }
            else
            {
                intPhoneNumber = 000000000;
                return intPhoneNumber;
            }
        }

        private Employees CreateEmployeeObject()
        {
            return new Employees
            {
                First_name = txtFirstName.Text,
                Last_name = txtLastName.Text,
                Job_title = (txtJobTitle.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Email = txtEmail.Text,
                Phone_number = ParsePhoneNumber(txtPhoneNumber.Text),
                Address = txtAdress.Text,
                Login = txtLogin.Text,
                Password = txtPassword.Text,
                Is_User_LoggedIn = false
            };
        }

        private void AddEmployee()
        {
            using (var dbContext = new AppDbContext())
            {
                Employees newEmployee = CreateEmployeeObject();
                dbContext.Employees.Add(newEmployee);
                dbContext.SaveChanges();
            }
        }

        private bool TryAddEmployee()
        {
            try
            {
                AddEmployee();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
