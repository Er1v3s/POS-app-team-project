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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockManagment : Page
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
        public StockManagment()
        {
            InitializeComponent();
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
    }
}
