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
using LiveCharts.Definitions.Charts;
using Org.BouncyCastle.Asn1.X509;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : Page
    {
        List<string> employeeNames = new List<string>();
        List<long> totalWorkTimes = new List<long>();

        //string[] raports = {"", "", "", "" };

        Dictionary<int, string> raports = new Dictionary<int, string>()
        {
            { 0, "Raport sprzedaży produktów" },
            { 1, "Raport zamówień" },
            { 2, "Czas pracy pracowników" },
            { 3, "Produktywność pracowników" },
            { 4, "Popularność produktów" }
        };

        public ReportsAndAnalysis()
        {
            InitializeComponent();
        }

        private void GenerateRaport_ButtonClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedComboBoxItem = (ComboBoxItem)reportTypeComboBox.SelectedItem;
            string selectedReport = selectedComboBoxItem.Content.ToString();
            DateTime startDate = datePickerFrom.SelectedDate.GetValueOrDefault();
            DateTime endDate = datePickerTo.SelectedDate.GetValueOrDefault();

            if (selectedReport == null)
            {
                MessageBox.Show("Nie wybrano typu raportu");
            }
            else if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                MessageBox.Show("Nie wybrano zakresu czasu");
            }
            else if (endDate < startDate)
            {
                MessageBox.Show("Data 'Do' nie może być wcześniejsza niż data 'Od'");
            }
            else
            {
                GenerateChoosenReport(selectedReport, startDate, endDate);
            }
        }

        private void GenerateChoosenReport(string selectedReport, DateTime startDate, DateTime endDate)
        {
            liveChart.Children.Clear();

            if (selectedReport == raports[0])
            {
                //GenerateSalesByProductReport(startDate, endDate);
                return;
            }
            else if (selectedReport == raports[1])
            {
                //GenerateOrdersReport(startDate, endDate);
                return;
            }
            else if (selectedReport == raports[2])
            {
                GenerateEmployeesWorkTimeReportChart(GenerateWorkingTimeData(startDate, endDate));
                return;
            }
            else if (selectedReport == raports[3])
            {
                GenerateEmployeeProductivityChart(GenerateEmployeeProductivityData(startDate, endDate));
            }
            else if (selectedReport == raports[4])
            {
                GenerateProductPopularityChart(GenerateProductPopularityData(startDate, endDate));
            }
        }

        // Nie działa
        #region Sales by Product

        //private void GenerateSalesByProductReport(DateTime startDate, DateTime endDate)
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        var salesData = from o in dbContext.Orders
        //                        join oi in dbContext.OrderItems on o.Order_id equals oi.Order_id
        //                        join p in dbContext.Products on oi.Product_id equals p.Product_id
        //                        where o.Order_time >= startDate && o.Order_time <= endDate
        //                        select new
        //                        {
        //                            OrderId = o.Order_id,
        //                            ProductName = p.Product_name,
        //                            QuantitySold = oi.Quantity,
        //                            PricePerUnit = p.Price,
        //                            TotalAmount = oi.Quantity * (p.Price ?? 0)
        //                        };

        //        var salesByProduct = salesData.GroupBy(s => s.ProductName)
        //        .Select(g => new
        //            {
        //                ProductName = g.Key,
        //                TotalQuantitySold = g.Sum(s => s.QuantitySold),
        //                TotalRevenue = g.Sum(s => s.TotalAmount)
        //            })
        //            .OrderByDescending(s => s.TotalRevenue)
        //            .ToList();

        //        SeriesCollection series = new SeriesCollection();
        //        List<string> labels = new List<string>();
        //        ChartValues<double> chartValues = new ChartValues<double>();

        //        foreach (var sale in salesByProduct)
        //        {
        //            labels.Add(sale.ProductName);
        //            chartValues.Add(sale.TotalQuantitySold);
        //        }

        //        series.Add(new ColumnSeries
        //        {
        //            Title = "Sales by Product",
        //            Values = chartValues
        //        });

        //        SalesChart.Series = series;

        //        SalesChart.AxisX.Add(new Axis
        //        {
        //            Title = "Products",
        //            Labels = labels
        //        });

        //        SalesChart.AxisY.Add(new Axis
        //        {
        //            Title = "Quantity"
        //        });

        //        SalesChart.LegendLocation = LegendLocation.Right;
        //    }
        //}

        #endregion

        // Nie działa
        #region Orders

        //private void GenerateOrdersReport(DateTime startDate, DateTime endDate)
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        var orders = dbContext.Orders
        //            .Where(order => order.Order_time >= startDate && order.Order_time <= endDate)
        //            .ToList();

        //        int ordersCount = orders.Count;

        //        double totalQuantity = orders
        //            .Join(dbContext.OrderItems, order => order.Order_id, orderItem => orderItem.Order_id, (order, orderItem) => orderItem)
        //            .Sum(orderItem => orderItem.Quantity);

        //        var productsAmount = orders
        //            .Join(dbContext.OrderItems, order => order.Order_id, orderItem => orderItem.Order_id, (order, orderItem) => new
        //            {
        //                orderItem.Quantity,
        //                orderItem.Product_id
        //            })
        //            .Join(dbContext.Products, orderItem => orderItem.Product_id, product => product.Product_id, (orderItem, product) => new
        //            {
        //                ProductName = product.Product_name,
        //                TotalAmount = orderItem.Quantity * (product.Price ?? 0)
        //            })
        //            .ToList();

        //        double totalAmount = productsAmount.Sum(item => item.TotalAmount);
        //        double averageOrderAmount = ordersCount > 0 ? totalAmount / ordersCount : 0;
        //        double averageOrderQuantity = ordersCount > 0 ? totalQuantity / ordersCount : 0;

        //        var salesData = productsAmount
        //            .GroupBy(item => item.ProductName)
        //            .Select(group => new
        //            {
        //                ProductName = group.Key,
        //                TotalSales = group.Sum(item => item.TotalAmount)
        //            })
        //            .ToList();

        //        var chartData = salesData.Select(item => item.TotalSales).ToList();

        //        SalesChart.Series.Add(new ColumnSeries
        //        {
        //            Title = "Raport zamówień",
        //            Values = new ChartValues<double>(chartData)
        //        });
        //    }
        //}

        #endregion

        // Jeszcze nie naprawione
        #region Working Time Raport

        private List<EmployeeWorkingTime> GenerateWorkingTimeData(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new AppDbContext())
            {
                var workTimeData = dbContext.EmployeeWorkSession
                    //.Where(session => session.WorkingTimeFrom >= startDate && session.WorkingTimeTo <= endDate)
                    .GroupBy(session => session.Employee_Id)
                    .Select(group => new
                    {
                        EmployeeId = group.Key,
                        TotalWorkTime = (long?)group.Sum(session => TimeSpan.Parse(session.Working_Time_Summary).Ticks)
                    })
                    .ToList();

                var raportData = workTimeData.Join(
                    dbContext.Employees,
                    workTime => workTime.EmployeeId,
                    employee => employee.Employee_id,
                    (workTime, employee) => new EmployeeWorkingTime
                    {
                        EmployeeName = $"{employee.First_name} {employee.Last_name}",
                        TotalWorkTime = (double)TimeSpan.FromTicks(workTime.TotalWorkTime ?? 0).TotalHours
                    })
                    .ToList();

                return raportData;
            }
        }


        //private long CalculateTotalWorkTime(string? fromTime, string? toTime)
        //{
        //    if (TimeSpan.TryParse(fromTime, out TimeSpan startTime) &&
        //        TimeSpan.TryParse(toTime, out TimeSpan endTime))
        //    {
        //        TimeSpan timeDifference = endTime - startTime;
        //        return timeDifference.Ticks;
        //    }

        //    return TimeSpan.Zero.Ticks;
        //}

        private void GenerateEmployeesWorkTimeReportChart(List<EmployeeWorkingTime> raportData)
        {
            var workingTimeChart = new CartesianChart();
            workingTimeChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Raport czasu pracy pracowników",
                    Values = new ChartValues<double>(raportData.Select(p => p.TotalWorkTime))
                }
            };

            workingTimeChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Pracownicy",
                Labels = raportData.Select(p => p.EmployeeName).ToList()
            });
        }

        #endregion

        #region Popularity of products

        private List<ProductPopularity> GenerateProductPopularityData(DateTime startDate, DateTime endDate)
        {
            List<ProductPopularity> productPopularityData;
            using (var dbContext = new AppDbContext())
            {
                productPopularityData = (from orderItems in dbContext.OrderItems
                                         join products in dbContext.Products on orderItems.Product_id equals products.Product_id
                                         join order in dbContext.Orders on orderItems.OrdersOrder_id equals order.Order_id
                                         where order.Order_time >= startDate && order.Order_time <= endDate
                                         group orderItems by products.Product_name into groupedItems
                                         select new ProductPopularity
                                         {
                                             ProductName = groupedItems.Key,
                                             Quantity = groupedItems.Sum(item => item.Quantity)
                                         }).ToList();
            }

            return productPopularityData;
        }

        private void GenerateProductPopularityChart(List<ProductPopularity> popularityOfProductsData)
        {
            var popularityOfProductsChart = new CartesianChart();

            popularityOfProductsChart.AxisY.Add(new Axis
            {
                LabelFormatter = value => Math.Floor(value).ToString(),
            });

            popularityOfProductsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Ilość sprzedanego produktu",
                    Values = new ChartValues<int>(popularityOfProductsData.Select(p => (p.Quantity))),
                    DataLabels = true,
                }
            };

            popularityOfProductsChart.AxisX.Add(new Axis
            {
                Labels = popularityOfProductsData.Select(p => p.ProductName).ToList(),
            });

            popularityOfProductsChart.LegendLocation = LegendLocation.Bottom;

            liveChart.Children.Add(popularityOfProductsChart);
        }

        #endregion

        #region Employee Productivity

        private List<EmployeeProductivity> GenerateEmployeeProductivityData(DateTime startDate, DateTime endDate)
        {
            List<EmployeeProductivity> productivityData;
            using (var dbContext = new AppDbContext())
            {
                productivityData = (from order in dbContext.Orders
                                    join employee in dbContext.Employees on order.Employee_id equals employee.Employee_id
                                    where order.Order_time >= startDate && order.Order_time <= endDate
                                    group order by new { employee.Employee_id, employee.First_name, employee.Last_name } into g
                                    select new EmployeeProductivity
                                    {
                                        EmployeeName = $"{g.Key.First_name} {g.Key.Last_name}",
                                        OrderCount = g.Count()
                                    }).ToList();
            }

            return productivityData;
        }

        private void GenerateEmployeeProductivityChart(List<EmployeeProductivity> productivityData)
        {
            var productivityChart = new CartesianChart();

            productivityChart.AxisY.Add(new Axis
            {
                LabelFormatter = value => Math.Floor(value).ToString(),
            });

            productivityChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Ilość zrealizowanych zamówień",
                    Values = new ChartValues<int>(productivityData.Select(p => p.OrderCount)),
                    DataLabels = true,
                }
            };

            productivityChart.AxisX.Add(new Axis
            {
                Labels = productivityData.Select(p => p.EmployeeName).ToList()
            });

            productivityChart.LegendLocation = LegendLocation.Bottom;

            liveChart.Children.Add(productivityChart);

        }

        #endregion

        public class ProductPopularity
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }

        public class EmployeeProductivity
        {
            public string EmployeeName { get; set; }
            public int OrderCount { get; set; }
        }

        public class EmployeeWorkingTime
        {
            public string EmployeeName { get; set; }
            public double TotalWorkTime { get; set; }
        }
    }
}