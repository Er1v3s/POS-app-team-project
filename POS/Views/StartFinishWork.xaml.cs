using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Logika interakcji dla klasy StartFinishWork.xaml
    /// </summary>
    public partial class StartFinishWork : UserControl
    {
        private readonly int employeeId;
        public StartFinishWork(int employeeId)
        {
            this.employeeId = employeeId;
            InitializeComponent();

            using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                if (user != null)
                {
                    employeeName.Text = user.First_name + " " + user.Last_name;
                    if(user.Is_User_LoggedIn)
                    {
                        StartWork.IsEnabled = false;
                    }
                    else
                    {
                         FinishWork.IsEnabled = false;
                    }
                }
            }
        }

        private async void StartWork_Button(object sender, RoutedEventArgs e)
        {
            await using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                if (user != null)
                {
                    user.Is_User_LoggedIn = true;
                    dbContext.SaveChanges();
                }
            }
        }

        private async void FinishWork_Button(object sender, RoutedEventArgs e)
        {
            await using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                if (user != null)
                {
                    user.Is_User_LoggedIn = false;
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
