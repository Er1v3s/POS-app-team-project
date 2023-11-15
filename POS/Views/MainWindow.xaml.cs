﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<Employee> employee = new ObservableCollection<Employee>();
            ObservableCollection<ToDoListTask> toDoListTasks = new ObservableCollection<ToDoListTask>();

            // Create working time summary DataGrid Item Info

            employee.Add(new Employee { firstName = "Andrzej",  workingTimeFrom = "18:00", workingTimeTo = "24:00", workingTimeSummary = "6:00"});           
            employee.Add(new Employee { firstName = "Łukasz", workingTimeFrom = "19:00", workingTimeTo = "23:00", workingTimeSummary = "4:00"});           
            employee.Add(new Employee { firstName = "Klara", workingTimeFrom = "20:00", workingTimeTo = "22:00", workingTimeSummary = "2:00"});           
            employee.Add(new Employee { firstName = "Mateusz", workingTimeFrom = "17:30", workingTimeTo = "22:30", workingTimeSummary = "5:00"});
            employee.Add(new Employee { firstName = "Robert", workingTimeFrom = "15:00", workingTimeTo = "20:00", workingTimeSummary = "5:00" });

            workingTimeSummaryDataGrid.ItemsSource = employee;

            // Create todo list DataGrid Item Info

            toDoListTasks.Add(new ToDoListTask { content = "Zrobić zamówienie" });
            toDoListTasks.Add(new ToDoListTask { content = "Wezwać serwis do nalewaka na piwo" });
            toDoListTasks.Add(new ToDoListTask { content = "Wysłać faktury do księgowej" });
            toDoListTasks.Add(new ToDoListTask { content = "Sprawdzić terminy ważności w lodówce" });
            toDoListTasks.Add(new ToDoListTask { content = "Wynieść śmieci" });

            todoListDataGrid.ItemsSource = toDoListTasks;
        }

        private void Move_To_Sales_Panel(object sender, RoutedEventArgs e)
        {
            Views.SalesPanel salesPanel = new Views.SalesPanel();
            salesPanel.Show();

            Window.GetWindow(this).Close();
        }
    }

    public class Employee
    {
        public string firstName { get; set; }
        public string workingTimeFrom { get; set; }
        public string workingTimeTo { get; set; }
        public string workingTimeSummary { get; set; }
    }

    public class ToDoListTask 
    {
        public string content { get; set; }
    }
}
