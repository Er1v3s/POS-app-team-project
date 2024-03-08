using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            employeesInfoDataGrid.SelectionChanged += EmployeesInfoDataGrid_SelectionChanged;
        }

        private void OpenAddEmployeeWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.ShowDialog();
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
                        switch (employee.Job_title)
                        {
                            case "Barman":
                                employeeInfo.Permission_level = 3;
                                break;
                            case "Miksolog":
                                employeeInfo.Permission_level = 4;
                                break;
                            case "Uczeń":
                                employeeInfo.Permission_level = 2;
                                break;
                            case "Kierownik":
                                employeeInfo.Permission_level = 5;
                                break;
                            default:
                                employeeInfo.Permission_level = 1; // Domyślny poziom uprawnień
                                break;
                        }
                        employeesCollection.Add(employeeInfo);
                    }
                }
            }

            employeesInfoDataGrid.ItemsSource = employeesCollection;
        }

        private void EditEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {
            EmployeeInfo selectedEmployee = employeesInfoDataGrid.SelectedItem as EmployeeInfo;

            if (selectedEmployee != null)
            {
                using (var dbContext = new AppDbContext())
                {
                    var employeeToEdit = dbContext.Employees
                        .FirstOrDefault(employee => employee.First_name + " " + employee.Last_name == selectedEmployee.Employee_name);

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

        private void EmployeesInfoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }
    }
}