using POS.Models;
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
                        var employee = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == order.EmployeeId);

                        OrderHistory formattedOrder = new OrderHistory()
                        {
                            OrderId = order.OrderId,
                            EmployeeName = employee.FirstName + " " + employee.LastName,
                            OrderDate = order.OrderTime.ToString("dd/MM/yyyy"),
                            OrderTime = order.OrderTime.ToString("HH:mm")
                        };

                        ordersHistoryDataGrid.Items.Add(formattedOrder);
                    }
                }
            }
        }
    }
}
