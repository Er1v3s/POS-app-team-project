using POS.Models;
using System.Windows;
using System.Windows.Controls;

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
            int phoneNumber;
            if (int.TryParse(txtPhoneNumber.Text, out phoneNumber))
            {
                phoneNumber = int.Parse(txtPhoneNumber.Text);
            }
            else
            {
                phoneNumber = 000000000;
            }
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
