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
using LiveCharts.Wpf;
using LiveCharts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : UserControl
    {
        List<string> employeeNames = new List<string>();
        List<long> totalWorkTimes = new List<long>();
        public ReportsAndAnalysis()
        {
            InitializeComponent();
        }

        private void GenerateRaport_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedComboBoxItem = (ComboBoxItem)reportTypeComboBox.SelectedItem;
            string selectedReport = selectedComboBoxItem.Content.ToString();
            DateTime fromDate = datePickerFrom.SelectedDate.GetValueOrDefault();
            DateTime toDate = datePickerTo.SelectedDate.GetValueOrDefault();

            if (selectedReport == null) 
            {
                MessageBox.Show("Nie wybrano typu raportu");
            }
            else if (fromDate == DateTime.MinValue || toDate == DateTime.MinValue)
            {
                MessageBox.Show("Nie wybrano zakresu czasu");
            }
            else if (toDate < fromDate)
            {
                MessageBox.Show("Data 'Do' nie może być wcześniejsza niż data 'Od'");
            } else
            {
                generateChoosenReport(selectedReport, fromDate, toDate);
            }
        }

        private void generateChoosenReport(string selectedReport , DateTime fromDate, DateTime toDate )
        {
            SalesChart.Series.Clear();

            if (selectedReport == "Raport sprzedaży produktów")
            {
                GenerateSalesByProductReport(fromDate, toDate);
            }
            else if (selectedReport == "Raport zamówień")
            {
                generateOrdersReport(fromDate, toDate);
            }
            else if (selectedReport == "Raport czasu pracy pracowników")
            {
                GenerateEmployeesWorkTimeReport(fromDate, toDate);
            }
        }
        
        private void GenerateSalesByProductReport(DateTime fromDate, DateTime toDate)
        {
            using (var dbContext = new AppDbContext())
            {
                var salesData = from o in dbContext.Orders
                                join oi in dbContext.OrderItems on o.Order_id equals oi.Order_id
                                join p in dbContext.Products on oi.Product_id equals p.Product_id
                                where o.Order_time >= fromDate && o.Order_time <= toDate
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
                                              .OrderByDescending(s => s.TotalRevenue)
                                              .ToList();

                SeriesCollection series = new SeriesCollection();
                List<string> labels = new List<string>();
                ChartValues<double> chartValues = new ChartValues<double>();

                foreach (var sale in salesByProduct)
                {
                    labels.Add(sale.ProductName);
                    chartValues.Add(sale.TotalQuantitySold);
                }

                series.Add(new ColumnSeries
                {
                    Title = "Sales by Product",
                    Values = chartValues
                });

                SalesChart.Series = series;

                SalesChart.AxisX.Add(new Axis
                {
                    Title = "Products",
                    Labels = labels
                });

                SalesChart.AxisY.Add(new Axis
                {
                    Title = "Quantity"
                });

                SalesChart.LegendLocation = LegendLocation.Right;
            }
        }
        private void generateOrdersReport(DateTime fromDate, DateTime toDate)
        {
            using (var dbContext = new AppDbContext())
            {
                var orders = dbContext.Orders
                    .Where(order => order.Order_time >= fromDate && order.Order_time <= toDate)
                    .ToList();

                int ordersCount = orders.Count;

                double totalQuantity = orders
                    .Join(dbContext.OrderItems, order => order.Order_id, orderItem => orderItem.Order_id, (order, orderItem) => orderItem)
                    .Sum(orderItem => orderItem.Quantity);

                var productsAmount = orders
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
                double averageOrderAmount = ordersCount > 0 ? totalAmount / ordersCount : 0;
                double averageOrderQuantity = ordersCount > 0 ? totalQuantity / ordersCount : 0;

                var salesData = productsAmount
                    .GroupBy(item => item.ProductName)
                    .Select(group => new
                    {
                        ProductName = group.Key,
                        TotalSales = group.Sum(item => item.TotalAmount)
                    })
                    .ToList();

                var chartData = salesData.Select(item => item.TotalSales).ToList();

                SalesChart.Series.Add(new ColumnSeries
                {
                    Title = "Raport zamówień",
                    Values = new ChartValues<double>(chartData)
                });
            }
        }

        private void GenerateEmployeesWorkTimeReport(DateTime fromDate, DateTime toDate)
        {
            using (var dbContext = new AppDbContext())
            {
                var employeesWorkSessions = dbContext.EmployeeWorkSession
                        .Where(session =>
                            session.Working_Time_From != null &&
                            session.Working_Time_To != null &&
                            session.Working_Time_From.CompareTo(fromDate.ToString()) >= 0 &&
                            session.Working_Time_To.CompareTo(toDate.ToString()) <= 0)
                        .ToList();

                var employeesWorkTime = employeesWorkSessions
                    .Where(session =>
                        DateTime.TryParse(session.Working_Time_From, out DateTime fromDateTime) &&
                        DateTime.TryParse(session.Working_Time_To, out DateTime toDateTime) &&
                        fromDateTime >= fromDate &&
                        toDateTime <= toDate)
                    .GroupBy(session => session.Employee_Id)
                    .Select(group => new
                    {
                        EmployeeId = group.Key,
                        TotalWorkTime = group.Sum(session =>
                            CalculateTotalWorkTime(session.Working_Time_From, session.Working_Time_To))
                    })
                    .ToList();

                foreach (var employeeWorkTime in employeesWorkTime)
                {
                    var employee = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeWorkTime.EmployeeId);
                    if (employee != null)
                    {
                        employeeNames.Add($"{employee.First_name} {employee.Last_name}");
                        totalWorkTimes.Add(employeeWorkTime.TotalWorkTime);
                    }
                }
            }

            SalesChart.Series.Add(new ColumnSeries
            {
                Title = "Raport czasu pracy pracowników",
                Values = new ChartValues<long>(totalWorkTimes)
            });

            SalesChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Pracownicy",
                Labels = employeeNames
            });
        }

        private long CalculateTotalWorkTime(string? fromTime, string? toTime)
        {
            if (TimeSpan.TryParse(fromTime, out TimeSpan startTime) &&
                TimeSpan.TryParse(toTime, out TimeSpan endTime))
            {
                TimeSpan timeDifference = endTime - startTime;
                return timeDifference.Ticks;
            }

            return TimeSpan.Zero.Ticks;
        }


    }
}