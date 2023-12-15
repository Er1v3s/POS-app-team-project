using POS.Converter;
using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockManagment
    {
        private Employees currentUser;
        public int EmployeeId;
        public StockManagment(int employeeId)
        {
            InitializeComponent();

            using (var dbContext = new AppDbContext())
            {
                currentUser = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
            }
            string welcomeMessage = $"{currentUser.First_name} {currentUser.Last_name}";
            SetWelcomeMessage(welcomeMessage);
            EmployeeId = employeeId;
        }
        private void MoveToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }
        private void SetWelcomeMessage(string message)
        {
            welcomeLabel.Content = message;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }
    }
}
