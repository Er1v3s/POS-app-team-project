using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Logika interakcji dla klasy AdministratorFuncions.xaml
    /// </summary>
    public partial class AdministratorFuncions : Page
    {
        ObservableCollection<EmployeeInfo> employeesCollection = new ObservableCollection<EmployeeInfo>();
        public AdministratorFuncions()
        {
            InitializeComponent();
            ShowEmployeesList();
        }

        private void ShowEmployeesList()
        {
            employeesCollection.Clear();

            using (var dbContext = new AppDbContext())
            {
                var employees = dbContext.Employees.ToList();

                if (employees != null)
                {
                    foreach (var employee in employees)
                    {
                        EmployeeInfo employeeInfo = new EmployeeInfo();
                        employeeInfo.Employee_name = employee.First_name + " " + employee.Last_name;
                        employeeInfo.Job_title = employee.Job_title;
                        employeeInfo.Permission_level = 5; // temporary data
                        employeesCollection.Add(employeeInfo);
                    }
                }
            }

            employeesInfoDataGrid.ItemsSource = employeesCollection;
        }
    }
}
