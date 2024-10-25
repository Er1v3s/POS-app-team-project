using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.EntityFrameworkCore;
using POS.ViewModel.Raports;
using POS.ViewModel.Reports;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : Page
    {
        Dictionary<int, string> reports = new Dictionary<int, string>()
        {
            { 0, "Raport sprzedaży produktów" },
            { 1, "Dzienny raport przychodów" },
            { 2, "Miesięczny raport przychodów" },
            { 3, "Roczny raport przychodów" },
            //{ 2, "Raport ilości zamówień" },
            //{ 3, "Raport ilości zamówień w konkretne dni tygodnia" },
            //{ 4, "Raport produktywności pracowników" },
            //{ 5, "Stosunek płatności kartą a gotówką" },

            //{ 2, "Raport zużycia materiałów" },
        };

        public ReportsAndAnalysis()
        {
            InitializeComponent();
        }

        private async void GenerateReport_ButtonClick(object sender, RoutedEventArgs e)
        {
            string selectedReport = null;
            ComboBoxItem selectedComboBoxItem = (ComboBoxItem)reportTypeComboBox.SelectedItem;
            if (selectedComboBoxItem != null)
            {
                selectedReport = selectedComboBoxItem.Content.ToString();
            }

            DateTime startDate = datePickerFrom.SelectedDate.GetValueOrDefault();
            DateTime endDate = datePickerTo.SelectedDate.GetValueOrDefault().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

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
                await GenerateChoosenReport(selectedReport, startDate, endDate);
            }
        }

        private async Task GenerateChoosenReport(string selectedReport, DateTime startDate, DateTime endDate)
        {
            liveChart.Children.Clear();

            if (selectedReport == reports[0])
            {
                List<ProductSales> productSalesData = await GenerateSalesReport(startDate, endDate);
                GenerateSalesReportChart(productSalesData);
            }
            else if (selectedReport == reports[1])
            {
                List<RevenueReport> revenueData = await GenerateRevenueReport(startDate, endDate, "Daily");
                GenerateRevenueChart(revenueData, "Przychód", "Data", p => p.Date.ToString("yyyy-MM-dd"));
            }
            else if (selectedReport == reports[2])
            {
                List<RevenueReport> revenueData = await GenerateRevenueReport(startDate, endDate, "Monthly");
                GenerateRevenueChart(revenueData, "Przychód", "Miesiąc", p => $"{p.Month:00}-{p.Year}");
            }
            else if (selectedReport == reports[3])
            {
                List<RevenueReport> revenueData = await GenerateRevenueReport(startDate, endDate, "Yearly");
                GenerateRevenueChart(revenueData, "Przychód", "Rok", p => p.Year.ToString());
            }
            //else if (selectedReport == raports[1])
            //{
            //    //await GenerateNumberOfOrdersOnSpecificDays(startDate, endDate);
            //}
            //else if (selectedReport == raports[2])
            //{
            //    GenerateConsumptionReport(startDate, endDate);
            //}
            //else if (selectedReport == raports[3])
            //{
            //    List <EmployeeProductivity> employeeProductivityData = GenerateEmployeeProductivityData(startDate, endDate);
            //    GenerateEmployeeProductivityChart(employeeProductivityData);
            //}
            //else if (selectedReport == raports[4])
            //{
            //    List<ProductPopularity> productPopularityData = GenerateProductPopularityData(startDate, endDate);
            //    GenerateProductPopularityChart(productPopularityData);
            //}
        }

        #region Sales raport

        private async Task<List<ProductSales>> GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            await using var dbContext = new AppDbContext();

            var productSales = await dbContext.OrderItems
                .Where(orderItem => dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .Select(order => order.OrderId)
                    .Contains(orderItem.OrderId))
                .GroupBy(orderItem => orderItem.ProductId)
                .Select(groupedItems => new ProductSales
                {
                    ProductName = dbContext.Products
                        .Where(product => product.ProductId == groupedItems.Key)
                        .Select(product => product.ProductName)
                        .FirstOrDefault(),
                    Quantity = groupedItems.Sum(item => item.Quantity)
                })
                .ToListAsync();

            return productSales;
        }

        private void GenerateSalesReportChart(List<ProductSales> productSales)
        {
            var salesProductsChart = new CartesianChart();

            salesProductsChart.AxisY.Add(new Axis
            {
                Title = "Ilość sprzedanych produktów",
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = (double)(productSales.Min(p => p.Quantity) * 0.8m),
                ShowLabels = false
            });

            salesProductsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Ilość sprzedanych produktów: ",
                    Values = new ChartValues<int>(productSales.Select(p => (p.Quantity))),
                    DataLabels = true,
                }
            };

            salesProductsChart.AxisX.Add(new Axis
            {
                Title = "Produkt",
                Labels = productSales.Select(p => p.ProductName).ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(salesProductsChart);
        }

        #endregion

        #region RevenueReports

        private async Task<List<RevenueReport>> GenerateRevenueReport(DateTime startDate, DateTime endDate, string groupBy)
        {
            await using var dbContext = new AppDbContext();

            var revenueReportQuery = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new { order.OrderTime, payment.Amount });

            IQueryable<RevenueReport> groupedQuery;

            switch (groupBy)
            {
                case "Daily":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Date)
                        .Select(g => new RevenueReport
                        {
                            Date = g.Key,
                            TotalRevenue = (decimal)g.Sum(x => x.Amount)
                        });
                    break;

                case "Monthly":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
                        .Select(g => new RevenueReport
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalRevenue = (decimal)g.Sum(x => x.Amount)
                        });
                    break;

                case "Yearly":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Year)
                        .Select(g => new RevenueReport
                        {
                            Year = g.Key,
                            TotalRevenue = (decimal)g.Sum(x => x.Amount)
                        });
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            return groupedQuery
                .AsEnumerable() // Switch to client-side evaluation
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ToList();
        }

        private void GenerateRevenueChart(List<RevenueReport> revenueReport, string title, string xAxisTitle, Func<RevenueReport, string> labelSelector)
        {
            var revenueChart = new CartesianChart();

            revenueChart.AxisY.Add(new Axis
            {
                Title = "Przychód",
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = (double)(revenueReport.Min(p => p.TotalRevenue) * 0.8m),
                ShowLabels = false
            });

            revenueChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = title,
                    Values = new ChartValues<decimal>(revenueReport.Select(p => p.TotalRevenue)),
                    DataLabels = true,
                }
            };

            revenueChart.AxisX.Add(new Axis
            {
                Title = xAxisTitle,
                Labels = revenueReport.Select(labelSelector).ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(revenueChart);
        }


        #endregion

        #region Consumption raport
        //private void GenerateConsumptionReport(DateTime startDate, DateTime endDate)
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        var consumptionReport = from orderItem in dbContext.OrderItems
        //            join order in dbContext.Orders on orderItem.OrderId equals order.OrderId
        //            where order.OrderTime >= startDate && order.OrderTime <= endDate
        //            join product in dbContext.Products on orderItem.ProductId equals product.ProductId
        //            join recipeIngredient in dbContext.RecipeIngredients on product.RecipeId equals recipeIngredient.RecipeId
        //            join ingredient in dbContext.Ingredients on recipeIngredient.IngredientId equals ingredient.IngredientId
        //            group new { orderItem, recipeIngredient } by new { ingredient.Name, ingredient.Unit } into grouped
        //            select new
        //            {
        //                IngredientName = grouped.Key.Name,
        //                Unit = grouped.Key.Unit,
        //                TotalConsumedQuantity = grouped.Sum(g => g.recipeIngredient.Quantity * g.orderItem.Quantity)
        //            };

        //        DataGrid consumptionReportDataGrid = new DataGrid();
        //        consumptionReportDataGrid.ItemsSource = consumptionReport.ToList();

        //        liveChart.Children.Add(consumptionReportDataGrid);
        //    }

        //}

        #endregion

        #region Employee productivity raport

        private List<EmployeeProductivity> GenerateEmployeeProductivityData(DateTime startDate, DateTime endDate)
        {
            List<EmployeeProductivity> productivityData;
            using (var dbContext = new AppDbContext())
            {
                productivityData = (from order in dbContext.Orders
                                    join employee in dbContext.Employees on order.EmployeeId equals employee.EmployeeId
                                    join payment in dbContext.Payments on order.OrderId equals payment.OrderId into payments
                                    where order.OrderTime >= startDate && order.OrderTime <= endDate
                                    group new { order, payments } by new { Employee_id = employee.EmployeeId, First_name = employee.FirstName, employee.LastName } into g
                                    select new EmployeeProductivity
                                    {
                                        EmployeeName = $"{g.Key.First_name} {g.Key.LastName}",
                                        OrderCount = g.Count(),
                                        TotalAmount = Math.Round(g.Sum(x => x.payments.Sum(p => p.Amount)), 2)
                                    }).ToList();
            }

            return productivityData;
        }

        private void GenerateEmployeeProductivityChart(List<EmployeeProductivity> productivityData)
        {
            var productivityChart = new CartesianChart();

            productivityChart.AxisY.Add(new Axis
            {
                Title = "Ilość zrealizowanych zamówień",
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            productivityChart.AxisY.Add(new Axis
            {
                Title = "Suma kwot zamówień",
                Position = AxisPosition.RightTop,
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            productivityChart.Series = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Ilość zrealizowanych zamówień: ",
            Values = new ChartValues<int>(productivityData.Select(p => p.OrderCount)),
            DataLabels = true,
        },
        new ColumnSeries
        {
            Title = "Suma kwot zamówień: ",
            Values = new ChartValues<double>(productivityData.Select(p => p.TotalAmount)),
            DataLabels = true,
            ScalesYAt = 1 // Ta seria będzie korzystać z drugiej osi Y
        }
    };

            productivityChart.AxisX.Add(new Axis
            {
                Title = "Pracownik",
                Labels = productivityData.Select(p => p.EmployeeName).ToList()
            });

            liveChart.Children.Add(productivityChart);
        }



        #endregion

        #region NumberOfOrdersOnSpecificDays

        private async Task GenerateNumberOfOrdersOnSpecificDays(DateTime startDate, DateTime endDate)
        {
            await using (var dbContext = new AppDbContext())
            {
                var ordersReport = await dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .GroupBy(order => order.DayOfWeek)
                    .Select(group => new
                    {
                        DayOfWeek = group.Key,
                        OrderCount = group.Count()
                    })
                    .ToListAsync();
                
                DataGrid ordersReportDataGrid = new DataGrid();
                ordersReportDataGrid.ItemsSource = ordersReport;

                liveChart.Children.Add(ordersReportDataGrid);
            }
        }

        #endregion
    }
}