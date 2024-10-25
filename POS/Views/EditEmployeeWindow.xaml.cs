using POS.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy EditEmployeeWindow.xaml
    /// </summary>
    public partial class EditEmployeeWindow : Window
    {
        private readonly Employees selectedEmployee;

        public EditEmployeeWindow(Employees selectedEmployee)
        {
            InitializeComponent();

            this.selectedEmployee = selectedEmployee;
            InsertCurrentEmployeeData();
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
            this.Close();
        }

        private void EditEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEmployee();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertCurrentEmployeeData()
        {
            using (var dbContext = new AppDbContext())
            {
                var employeeToUpdate = dbContext.Employees.FirstOrDefault(employee => employee.EmployeeId == this.selectedEmployee.EmployeeId);

                txtFirstName.Text = employeeToUpdate.FirstName != null ? employeeToUpdate.FirstName : "";
                txtLastName.Text = employeeToUpdate.LastName != null ? employeeToUpdate.LastName : "";
                txtJobTitle.Text = employeeToUpdate.JobTitle != null ? employeeToUpdate.JobTitle : "";
                txtEmail.Text = employeeToUpdate.Email != null ? employeeToUpdate.Email : "";
                txtPhoneNumber.Text = employeeToUpdate.PhoneNumber != null ? employeeToUpdate.PhoneNumber.ToString() : "";
                txtAdress.Text = employeeToUpdate.Address != null ? employeeToUpdate.Address : " ";
                txtLogin.Text = employeeToUpdate.Login;
                txtPassword.Text = employeeToUpdate.Password;
            }
        }

        private int ParsePhoneNumber(string txtPhoneNumber)
        {
            int intPhoneNumber;

            if (int.TryParse(txtPhoneNumber, out intPhoneNumber))
            {
                return intPhoneNumber;
            }
            else
            {
                intPhoneNumber = 000000000;
                return intPhoneNumber;
            }
        }

        private void UpdateEmployee()
        {
            using (var dbContext = new AppDbContext())
            {
                var employeeToUpdate = dbContext.Employees.FirstOrDefault(employee => employee.EmployeeId == this.selectedEmployee.EmployeeId);

                if (employeeToUpdate != null)
                {
                    employeeToUpdate.FirstName = FormValidatorHelper.ValidateTextBox(txtFirstName);
                    employeeToUpdate.LastName = FormValidatorHelper.ValidateTextBox(txtLastName);
                    employeeToUpdate.JobTitle = FormValidatorHelper.ValidateComboBox(txtJobTitle);
                    employeeToUpdate.Email = FormValidatorHelper.ValidateEmailAddress(txtEmail);
                    employeeToUpdate.PhoneNumber = ParsePhoneNumber(FormValidatorHelper.ValidatePhoneNumber(txtPhoneNumber));
                    employeeToUpdate.Address = FormValidatorHelper.ValidateTextBox(txtAdress);
                    employeeToUpdate.Login = FormValidatorHelper.ValidateTextBox(txtLogin);
                    employeeToUpdate.Password = FormValidatorHelper.ValidateTextBox(txtPassword);
                }

                dbContext.SaveChanges();
            }
        }

        private async void FormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidateTextBox(sender, e);
        }
        private void EmailFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidateEmailAddress(sender, e);
        }

        private void PhoneNumberFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidatePhoneNumber(sender, e);
        }
    }
}
