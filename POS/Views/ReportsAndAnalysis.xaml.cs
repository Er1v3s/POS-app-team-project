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
using LiveCharts.Defaults;
using System.Collections.ObjectModel;
using POS.Migrations;
using POS.ViewModel;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : UserControl
    {
        public ReportsAndAnalysis()
        {
            InitializeComponent();
        }

        private void generateSalesByProductReport()
        {
            using (var dbContext = new AppDbContext())
            {
                var salesData = from o in dbContext.Orders
                                join oi in dbContext.OrderItems on o.Order_id equals oi.Order_id
                                join p in dbContext.Products on oi.Product_id equals p.Product_id
                                select new
                                {
                                    OrderId = o.Order_id,
                                    ProductName = p.Product_name,
                                    QuantitySold = oi.Quantity,
                                    PricePerUnit = p.Price,
                                    TotalAmount = oi.Quantity * (p.Price ?? 0)
                                };
                var salesByProduct = salesData.GroupBy(s => s.ProductName)
                                          .Select(g => new
                                          {
                                              ProductName = g.Key,
                                              TotalQuantitySold = g.Sum(s => s.QuantitySold),
                                              TotalRevenue = g.Sum(s => s.TotalAmount)
                                          })
                                          .OrderByDescending(s => s.TotalRevenue);
            }
        }
        private void generateOrdersReport(DateTime date)
        {
            using (var dbContext = new AppDbContext())
            {
                int ordersCount = dbContext.Orders.Count(order => order.Order_time.Date == date.Date);

                double totalQuantity = dbContext.Orders
                    .Where(order => order.Order_time.Date == date.Date)
                    .Join(dbContext.OrderItems, order => order.Order_id, orderItem => orderItem.Order_id, (order, orderItem) => orderItem)
                    .Sum(orderItem => orderItem.Quantity);
                var productsAmount = dbContext.Orders
                    .Where(order => order.Order_time.Date == date.Date)
                    .Join(dbContext.OrderItems, order => order.Order_id, orderItem => orderItem.Order_id, (order, orderItem) => new
                    {
                        orderItem.Quantity,
                        orderItem.Product_id
                    })
                    .Join(dbContext.Products, orderItem => orderItem.Product_id, product => product.Product_id, (orderItem, product) => new
                    {
                        ProductName = product.Product_name,
                        TotalAmount = orderItem.Quantity * (product.Price ?? 0)
                    })
                    .ToList();

                double totalAmount = productsAmount.Sum(item => item.TotalAmount);
                double averageOrderAmount = totalAmount / ordersCount;
                double averageOrderQuantity = totalQuantity / ordersCount;
            }
        }

        private void GenerateEmployeesWorkTimeReport()
        {
            using (var dbContext = new AppDbContext())
            {
                var workSessions = dbContext.EmployeeWorkSession.ToList();
                var groupedSessions = workSessions
                    .GroupBy(session => session.Employee_Id)
                    .ToDictionary(group => group.Key, group => group.ToList());

                var employeesWorkTime = from sessionGroup in groupedSessions
                                        join employee in dbContext.Employees on sessionGroup.Key equals employee.Employee_id
                                        select new
                                        {
                                            Employee = employee,
                                            TotalWorkTime = CalculateTotalWorkTime(sessionGroup.Value)
                                        };
            }
        }

        private TimeSpan CalculateTotalWorkTime(List<EmployeeWorkSession> sessions)
        {
            TimeSpan totalWorkTime = TimeSpan.Zero;
            foreach (var session in sessions)
            {
                if (TimeSpan.TryParse(session.Working_Time_Summary, out TimeSpan sessionTime))
                {
                    totalWorkTime += sessionTime;
                }
            }

            return totalWorkTime;
        }


    }
}
