using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Wpf;
using LiveCharts;
using POS.ViewModel.Raports;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : Page
    {
        Dictionary<int, string> raports = new Dictionary<int, string>()
        {
            { 0, "Raport sprzedaży produktów" },
            { 1, "Raport zużycia materiałów" },
            { 2, "Produktywność pracowników" },
            { 3, "Popularność produktów" }
        };

        public ReportsAndAnalysis()
        {
            InitializeComponent();
        }

        private void GenerateRaport_ButtonClick(object sender, RoutedEventArgs e)
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
                GenerateChoosenReport(selectedReport, startDate, endDate);
            }
        }

        private void GenerateChoosenReport(string selectedReport, DateTime startDate, DateTime endDate)
        {
            liveChart.Children.Clear();

            if (selectedReport == raports[0])
            {
                GenerateSalesReport(startDate, endDate);
            }
            else if (selectedReport == raports[1])
            {
                GenerateConsumptionReport(startDate, endDate);
            }
            else if (selectedReport == raports[2])
            {
                List <EmployeeProductivity> employeeProductivityData = GenerateEmployeeProductivityData(startDate, endDate);
                GenerateEmployeeProductivityChart(employeeProductivityData);
            }
            else if (selectedReport == raports[3])
            {
                List<ProductPopularity> productPopularityData = GenerateProductPopularityData(startDate, endDate);
                GenerateProductPopularityChart(productPopularityData);
            }
        }

        #region Sales raport

        private void GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new AppDbContext())
            {
                var salesReport = dbContext.Products
                .Select(product => new
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    TotalSales = dbContext.OrderItems
                        .Where(orderItem => orderItem.ProductId == product.ProductId
                                           && orderItem.OriderTime >= startDate
                                           && orderItem.OriderTime <= endDate)
                        .Sum(orderItem => orderItem.Quantity * product.Price),
                    TotalAmount = dbContext.OrderItems
                        .Where(orderItem => orderItem.ProductId == product.ProductId
                                           && orderItem.OriderTime >= startDate
                                           && orderItem.OriderTime <= endDate)
                        .Sum(o => o.Quantity)
                });

                DataGrid salesRaportDataGrid = new DataGrid();
                salesRaportDataGrid.ItemsSource = salesReport.ToList();

                liveChart.Children.Add(salesRaportDataGrid);
            }
        }

        #endregion

        #region Consumption raport
        private void GenerateConsumptionReport(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new AppDbContext())
            {

                var consumptionReport = from orderItem in dbContext.OrderItems
                                        where orderItem.OriderTime >= startDate && orderItem.OriderTime <= endDate
                                        join product in dbContext.Products on orderItem.ProductId equals product.ProductId
                                        join recipeIngredient in dbContext.RecipeIngredients on product.RecipeId equals recipeIngredient.RecipeId
                                        join ingredient in dbContext.Ingredients on recipeIngredient.IngredientId equals ingredient.IngredientId
                                        group new { orderItem, recipeIngredient } by new { ingredient.Name, ingredient.Unit } into grouped
                                        select new
                                        {
                                            IngredientName = grouped.Key.Name,
                                            Unit = grouped.Key.Unit,
                                            TotalConsumedQuantity = grouped.Sum(g => g.recipeIngredient.Quantity * g.orderItem.Quantity)
                                        };

                DataGrid consumptionReportDataGrid = new DataGrid();
                consumptionReportDataGrid.ItemsSource = consumptionReport.ToList();

                liveChart.Children.Add(consumptionReportDataGrid);
            }
        }

        #endregion

        #region Popularity of products raport

        private List<ProductPopularity> GenerateProductPopularityData(DateTime startDate, DateTime endDate)
        {
            List<ProductPopularity> productPopularityData;
            using (var dbContext = new AppDbContext())
            {
                productPopularityData = (from orderItems in dbContext.OrderItems
                                         join products in dbContext.Products on orderItems.ProductId equals products.ProductId
                                         join order in dbContext.Orders on orderItems.OrdersOrderId equals order.OrderId
                                         where order.OrderTime >= startDate && order.OrderTime <= endDate
                                         group orderItems by products.ProductName into groupedItems
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
                Title = "Ilość sprzedanych produktów",
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            popularityOfProductsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Ilość sprzedanych produktów: ",
                    Values = new ChartValues<int>(popularityOfProductsData.Select(p => (p.Quantity))),
                    DataLabels = true,
                }
            };

            popularityOfProductsChart.AxisX.Add(new Axis
            {
                Title = "Produkt",
                Labels = popularityOfProductsData.Select(p => p.ProductName).ToList(),
            });

            liveChart.Children.Add(popularityOfProductsChart);
        }

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
    }
}