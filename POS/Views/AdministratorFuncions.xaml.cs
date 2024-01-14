using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            employeesInfoDataGrid.SelectionChanged += employeesInfoDataGrid_SelectionChanged;
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

        private void OpenAddEditEmployeeWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            AddEditEmployeeWindow addEditEmployeeWindow = new AddEditEmployeeWindow();
            addEditEmployeeWindow.ShowDialog();
        }

        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeInfo selectedEmployee = employeesInfoDataGrid.SelectedItem as EmployeeInfo;

            if (selectedEmployee != null)
            {
                using (var dbContext = new AppDbContext())
                {
                    var employeeToEdit = dbContext.Employees
                        .FirstOrDefault(emp => emp.First_name + " " + emp.Last_name == selectedEmployee.Employee_name);

                    if (employeeToEdit != null)
                    {
                        EditEmployeeWindow editEmployeeWindow = new EditEmployeeWindow(employeeToEdit);
                        editEmployeeWindow.ShowDialog();
                    }
                }
            }
        }

        private void DeleteEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (employeesInfoDataGrid.SelectedItem != null)
            {
                EmployeeInfo selectedEmployee = employeesInfoDataGrid.SelectedItem as EmployeeInfo;

                MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz usunąć pracownika {selectedEmployee.Employee_name}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var dbContext = new AppDbContext())
                    {
                        var employeeToRemove = dbContext.Employees
                            .FirstOrDefault(emp => emp.First_name + " " + emp.Last_name == selectedEmployee.Employee_name);

                        if (employeeToRemove != null)
                        {
                            dbContext.Employees.Remove(employeeToRemove);
                            dbContext.SaveChanges();

                            ShowEmployeesList();
                        }
                    }
                }
            }
        }

        private void RefreshEmployeesList_ButtonClick(object sender, RoutedEventArgs e)
        {
            ShowEmployeesList();
        }

        private void employeesInfoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (employeesInfoDataGrid.SelectedItem != null)
            {
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        private void ShowCashBalance_ButtonClick(object sender, RoutedEventArgs e)
        {
            double cashBalance = 0;

            using(var dbContext = new AppDbContext())
            {
                cashBalance = dbContext.Payments
                                .Where(payment => payment.Payment_method == "Gotówka")
                                .Sum(payment => payment.Amount);
            }

            cashBalance = Math.Round(cashBalance, 2);

            MessageBox.Show($"Stan kasy wynosi: {cashBalance :C}");
        }
    }
}