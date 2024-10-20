﻿using POS.Models;
using POS.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy FinishedOrders.xaml
    /// </summary>
    public partial class FinishedOrders : Window
    {
        public FinishedOrders()
        {
            InitializeComponent();

            generateOrderHistory();
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

        private void generateOrderHistory()
        {
            using (var dbContext = new AppDbContext())
            {
                var orders = dbContext.Orders.ToList();
                if (orders != null)
                {
                    foreach(var order in orders)
                    {
                        var employee = dbContext.Employees.FirstOrDefault(e => e.Employee_id == order.Employee_id);

                        OrderHistory formattedOrder = new OrderHistory()
                        {
                            Order_Id = order.Order_id,
                            Employee_Name = employee.First_name + " " + employee.Last_name,
                            Order_Date = order.Order_time.ToString("dd/MM/yyyy"),
                            Order_Time = order.Order_time.ToString("HH:mm")
                        };

                        ordersHistoryDataGrid.Items.Add(formattedOrder);
                    }
                }
            }
        }
    }
}
