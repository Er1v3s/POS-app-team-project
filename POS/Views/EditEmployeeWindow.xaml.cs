using POS.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            lblFirstName.Content = $"(Aktualny: {selectedEmployee.First_name})";
            lblLastName.Content = $"(Aktualny: {selectedEmployee.Last_name})";
            lblJobTitle.Content = $"(Aktualny: {selectedEmployee.Job_title})";
            lblEmail.Content = $"(Aktualny: {selectedEmployee.Email})";
            lblPhoneNumber.Content = $"(Aktualny: {selectedEmployee.Phone_number})";
            lblAdress.Content = $"(Aktualny: {selectedEmployee.Address})";
            lblLogin.Content = $"(Aktualny: {selectedEmployee.Login})";
            lblPassword.Content = $"(Aktualny: {selectedEmployee.Password})";
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            string newFirstName = txtFirstName.Text;
            string newLastName = txtLastName.Text;
            string newJobTitle = (txtJobTitle.SelectedItem as ComboBoxItem)?.Content.ToString();
            string newEmail = txtEmail.Text;
            int newPhoneNumber;
            if (int.TryParse(txtPhoneNumber.Text, out newPhoneNumber))
            {
                newPhoneNumber = int.Parse(txtPhoneNumber.Text);
            }
            else
            {
                newPhoneNumber = 000000000;
            }
            string newAddress = txtAdress.Text;
            string newLogin = txtLogin.Text;
            string newPassword = txtPassword.Text;

            using (var dbContext = new AppDbContext())
            {
                var employeeToUpdate = dbContext.Employees.FirstOrDefault(emp => emp.First_name == selectedEmployee.First_name && emp.Last_name == selectedEmployee.Last_name);

                if (employeeToUpdate != null)
                {
                    employeeToUpdate.First_name = newFirstName;
                    employeeToUpdate.Last_name = newLastName;
                    employeeToUpdate.Job_title = newJobTitle;
                    employeeToUpdate.Email = newEmail;
                    employeeToUpdate.Phone_number = newPhoneNumber;
                    employeeToUpdate.Address = newAddress;
                    employeeToUpdate.Login = newLogin;
                    employeeToUpdate.Password = newPassword;

                    dbContext.SaveChanges();
                }
            }
            this.Close();
        }
    }
}
