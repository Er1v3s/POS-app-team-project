using POS.Helpers;
using POS.Models;
using System;
using System.Windows;
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
            try
            {
                AddEmployee();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                First_name = FormValidatorHelper.ValidateTextBox(txtFirstName),
                Last_name = FormValidatorHelper.ValidateTextBox(txtLastName),
                Job_title = FormValidatorHelper.ValidateComboBox(txtJobTitle),
                Email = FormValidatorHelper.ValidateEmailAddress(txtEmail),
                Phone_number = ParsePhoneNumber(FormValidatorHelper.ValidatePhoneNumber(txtPhoneNumber)),
                Address = FormValidatorHelper.ValidateTextBox(txtAdress),
                Login = FormValidatorHelper.ValidateTextBox(txtLogin),
                Password = FormValidatorHelper.ValidateTextBox(txtPassword),
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

        private void FormInput_LostFocus(object sender, RoutedEventArgs e)
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
