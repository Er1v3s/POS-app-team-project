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
        List<string> employeeNames = new List<string>();
        List<long> totalWorkTimes = new List<long>();

        Dictionary<int, string> raports = new Dictionary<int, string>()
        {
            { 0, "Raport sprzedaży produktów" },
            { 1, "Raport zużycia materiałów" },
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
                List<EmployeeWorkingTime> workingTimeData = GenerateWorkingTimeData(startDate, endDate);
                GenerateEmployeesWorkTimeReportChart(workingTimeData);
            }
            else if (selectedReport == raports[3])
            {
                List <EmployeeProductivity> employeeProductivityData = GenerateEmployeeProductivityData(startDate, endDate);
                GenerateEmployeeProductivityChart(employeeProductivityData);
            }
            else if (selectedReport == raports[4])
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
                    ProductId = product.Product_id,
                    ProductName = product.Product_name,
                    TotalSales = dbContext.OrderItems
                        .Where(orderItem => orderItem.Product_id == product.Product_id
                                           && orderItem.Orider_time >= startDate
                                           && orderItem.Orider_time <= endDate)
                        .Sum(orderItem => orderItem.Quantity * product.Price),
                    TotalAmount = dbContext.OrderItems
                        .Where(orderItem => orderItem.Product_id == product.Product_id
                                           && orderItem.Orider_time >= startDate
                                           && orderItem.Orider_time <= endDate)
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
                                        where orderItem.Orider_time >= startDate && orderItem.Orider_time <= endDate
                                        join product in dbContext.Products on orderItem.Product_id equals product.Product_id
                                        join recipeIngredient in dbContext.RecipeIngredients on product.Recipe_id equals recipeIngredient.Recipe_id
                                        join ingredient in dbContext.Ingredients on recipeIngredient.Ingredient_id equals ingredient.Ingredient_id
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

        // Nie działa
        #region Working time Raport

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

            liveChart.Children.Add(workingTimeChart);
        }

        #endregion

        #region Popularity of products raport

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
                Title = "Ilość zrealizowanych zamówień",
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