using POS.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            TryUpdateEmployee();
            this.Close();
        }

        private void InsertCurrentEmployeeData()
        {
            using (var dbContext = new AppDbContext())
            {
                var employeeToUpdate = dbContext.Employees.FirstOrDefault(employee => employee.Employee_id == this.selectedEmployee.Employee_id);

                txtFirstName.Text = employeeToUpdate.First_name != null ? employeeToUpdate.First_name : "";
                txtLastName.Text = employeeToUpdate.Last_name != null ? employeeToUpdate.Last_name : "";
                txtJobTitle.Text = employeeToUpdate.Job_title != null ? employeeToUpdate.Job_title : "";
                txtEmail.Text = employeeToUpdate.Email != null ? employeeToUpdate.Email : "";
                txtPhoneNumber.Text = employeeToUpdate.Phone_number != null ? employeeToUpdate.Phone_number.ToString() : "";
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
                var employeeToUpdate = dbContext.Employees.FirstOrDefault(employee => employee.Employee_id == this.selectedEmployee.Employee_id);

                if (employeeToUpdate != null)
                {
                    employeeToUpdate.First_name = txtFirstName.Text;
                    employeeToUpdate.Last_name = txtLastName.Text;
                    employeeToUpdate.Job_title = txtJobTitle.Text;
                    employeeToUpdate.Email = txtEmail.Text;
                    employeeToUpdate.Phone_number = ParsePhoneNumber(txtPhoneNumber.Text);
                    employeeToUpdate.Address = txtAdress.Text;
                    employeeToUpdate.Login = txtLogin.Text;
                    employeeToUpdate.Password = txtPassword.Text;
                }

                dbContext.SaveChanges();
            }
        }

        private bool TryUpdateEmployee()
        {
            try
            {
                UpdateEmployee();
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
