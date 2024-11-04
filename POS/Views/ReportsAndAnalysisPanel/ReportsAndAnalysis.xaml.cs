using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.EntityFrameworkCore;
using Separator = LiveCharts.Wpf.Separator;
using POS.Models.Reports;

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
            { 4, "Dzienny raport ilości zamówień" },
            { 5, "Miesięczny raport ilości zamówień" },
            { 6, "Roczny raport ilości zamówień" },
            { 7, "Raport ilości zamówień w konkretne dni tygodnia" },
            { 8, "Raport produktywności pracowników" },
            { 9, "Stosunek płatności kartą a gotówką" },
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
            DateTime endDate = datePickerTo.SelectedDate.GetValueOrDefault().Date.AddHours(23).AddMinutes(59)
                .AddSeconds(59);

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
                List<ProductSalesDto> productSalesData = await GenerateSalesReport(startDate, endDate);
                GenerateSalesReportChart(productSalesData);
            }
            else if (selectedReport == reports[1])
            {
                List<RevenueReportDto> revenueData = await GenerateRevenueReport(startDate, endDate, "Daily");
                GenerateRevenueChart(revenueData, "Przychód", "Data", p => p.Date.ToString("yyyy-MM-dd"));
            }
            else if (selectedReport == reports[2])
            {
                List<RevenueReportDto> revenueData = await GenerateRevenueReport(startDate, endDate, "Monthly");
                GenerateRevenueChart(revenueData, "Przychód", "Miesiąc", p => $"{p.Month:00}-{p.Year}");
            }
            else if (selectedReport == reports[3])
            {
                List<RevenueReportDto> revenueData = await GenerateRevenueReport(startDate, endDate, "Yearly");
                GenerateRevenueChart(revenueData, "Przychód", "Rok", p => p.Year.ToString());
            }
            else if (selectedReport == reports[4])
            {
                List<OrderReportDto> orderReports = await GenerateNumberOfOrdersOnDays(startDate, endDate);
                GenerateOrdersChartForDays(orderReports);
            }
            else if (selectedReport == reports[5])
            {
                List<OrderReportDto> orderReports = await GenerateNumberOfOrdersByMonths(startDate, endDate);
                GenerateOrdersChartForMonths(orderReports);
            }
            else if (selectedReport == reports[6])
            {
                List<OrderReportDto> orderReports = await GenerateNumberOfOrdersByYears(startDate, endDate);
                GenerateOrdersChartForYears(orderReports);
            }
            else if (selectedReport == reports[7])
            {
                List<OrderReportDto> orderReports = await GenerateNumberOfOrdersOnSpecificDays(startDate, endDate);
                GenerateOrdersChart(orderReports);
            }
            else if (selectedReport == reports[8])
            {
                List<EmployeeProductivityDto> employeeProductivityData =
                    await GenerateEmployeeProductivityData(startDate, endDate);
                GenerateEmployeeProductivityChart(employeeProductivityData);
            }
            else if (selectedReport == reports[9])
            {
                List<PaymentRatioDto> paymentRatio = await GenerateCardToCashPaymentRatioData(startDate, endDate);
                GeneratePaymentMethodChart(paymentRatio);
            }
        }

        #region Sales raport

        private async Task<List<ProductSalesDto>> GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            await using var dbContext = new AppDbContext();

            var productSales = await dbContext.OrderItems
                .Where(orderItem => dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .Select(order => order.OrderId)
                    .Contains(orderItem.OrderId))
                .GroupBy(orderItem => orderItem.ProductId)
                .Select(groupedItems => new ProductSalesDto
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

        private void GenerateSalesReportChart(List<ProductSalesDto> productSales)
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

        private async Task<List<RevenueReportDto>> GenerateRevenueReport(DateTime startDate, DateTime endDate,
            string groupBy)
        {
            await using var dbContext = new AppDbContext();

            var revenueReportQuery = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new { order.OrderTime, payment.Amount });

            IQueryable<RevenueReportDto> groupedQuery;

            switch (groupBy)
            {
                case "Daily":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Date)
                        .Select(g => new RevenueReportDto
                        {
                            Date = g.Key,
                            TotalRevenue = (decimal)g.Sum(x => x.Amount)
                        });
                    break;

                case "Monthly":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
                        .Select(g => new RevenueReportDto
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalRevenue = (decimal)g.Sum(x => x.Amount)
                        });
                    break;

                case "Yearly":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Year)
                        .Select(g => new RevenueReportDto
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

        private void GenerateRevenueChart(List<RevenueReportDto> revenueReport, string title, string xAxisTitle,
            Func<RevenueReportDto, string> labelSelector)
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

        #region NumberOfOrdersReports

        #region NumberOfOrdersOnSpecificDays

        private async Task<List<OrderReportDto>> GenerateNumberOfOrdersOnSpecificDays(DateTime startDate, DateTime endDate)
        {
            await using (var dbContext = new AppDbContext())
            {
                var ordersReport = await dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .GroupBy(order => order.DayOfWeek)
                    .Select(group => new OrderReportDto
                    {
                        DayOfWeek = group.Key,
                        OrderCount = group.Count()
                    })
                    .OrderBy(order => order.DayOfWeek)
                    .ToListAsync();

                return ordersReport;
            }
        }

        private void GenerateOrdersChart(List<OrderReportDto> ordersReport)
        {
            var ordersChart = new CartesianChart();

            ordersChart.AxisY.Add(new Axis
            {
                Title = "Liczba zamówień",
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
            });

            var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

            ordersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Zamówienia",
                    Values = chartValues,
                    DataLabels = true,
                }
            };

            ordersChart.AxisX.Add(new Axis
            {
                Title = "Dzień tygodnia",
                Labels = ordersReport.Select(o => o.DayOfWeek.ToString()).ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(ordersChart);
        }


        #endregion

        #region NumberOfOrdersOnDays

        private async Task<List<OrderReportDto>> GenerateNumberOfOrdersOnDays(DateTime startDate, DateTime endDate)
        {
            await using (var dbContext = new AppDbContext())
            {
                var ordersReport = dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .GroupBy(order => order.OrderTime.Date)
                    .Select(group => new OrderReportDto
                    {
                        Date = group.Key,
                        OrderCount = group.Count()
                    })
                    .AsEnumerable()
                    .OrderBy(order => order.Date.DayOfWeek)
                    .ToList();

                return ordersReport;
            }
        }

        private void GenerateOrdersChartForDays(List<OrderReportDto> ordersReport)
        {
            var ordersChart = new CartesianChart();

            ordersChart.AxisY.Add(new Axis
            {
                Title = "Liczba zamówień",
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
            });

            var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

            ordersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Zamówienia",
                    Values = chartValues,
                    DataLabels = true,
                }
            };

            ordersChart.AxisX.Add(new Axis
            {
                Title = "Data",
                Labels = ordersReport.Select(o => o.Date.ToString("yyyy-MM-dd")).ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(ordersChart);
        }

        #endregion

        #region NumberOfOrdersByMonths

        private async Task<List<OrderReportDto>> GenerateNumberOfOrdersByMonths(DateTime startDate, DateTime endDate)
        {
            await using (var dbContext = new AppDbContext())
            {
                var ordersReport = dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                    .Select(group => new OrderReportDto
                    {
                        Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                        OrderCount = group.Count()
                    })
                    .AsEnumerable()
                    .OrderBy(order => order.Date.DayOfWeek)
                    .ToList();

                return ordersReport;
            }
        }


        private void GenerateOrdersChartForMonths(List<OrderReportDto> ordersReport)
        {
            var ordersChart = new CartesianChart();

            ordersChart.AxisY.Add(new Axis
            {
                Title = "Liczba zamówień",
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
            });

            var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

            ordersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Zamówienia",
                    Values = chartValues,
                    DataLabels = true,
                }
            };

            ordersChart.AxisX.Add(new Axis
            {
                Title = "Miesiąc",
                Labels = ordersReport.Select(o => new DateTime(1, 1, (int)o.DayOfWeek + 1).ToString("MMMM yyyy"))
                    .ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(ordersChart);
        }

        #endregion

        #region NumberOfOrdersByYears

        private async Task<List<OrderReportDto>> GenerateNumberOfOrdersByYears(DateTime startDate, DateTime endDate)
        {
            await using (var dbContext = new AppDbContext())
            {
                var ordersReport = dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .GroupBy(order => order.OrderTime.Year)
                    .AsEnumerable()
                    .Select(group => new OrderReportDto
                    {
                        Date = new DateTime(group.Key, 1, 1),
                        OrderCount = group.Count()
                    })
                    .OrderBy(order => order.Date.DayOfWeek)
                    .ToList();

                return ordersReport;
            }
        }


        private void GenerateOrdersChartForYears(List<OrderReportDto> ordersReport)
        {
            var ordersChart = new CartesianChart();

            ordersChart.AxisY.Add(new Axis
            {
                Title = "Liczba zamówień",
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
            });

            var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

            ordersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Zamówienia",
                    Values = chartValues,
                    DataLabels = true,
                }
            };

            ordersChart.AxisX.Add(new Axis
            {
                Title = "Rok",
                Labels = ordersReport.Select(o => o.Date.Year.ToString()).ToList(),
                IsEnabled = true
            });

            liveChart.Children.Add(ordersChart);
        }

        #endregion

        #endregion

        #region Employee productivity raport

        private async Task<List<EmployeeProductivityDto>> GenerateEmployeeProductivityData(DateTime startDate,
            DateTime endDate)
        {
            await using var dbContext = new AppDbContext();

            var productivityData = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Employees, order => order.EmployeeId, employee => employee.EmployeeId,
                    (order, employee) => new { order, employee })
                .Join(dbContext.Payments, orderEmployee => orderEmployee.order.OrderId, payment => payment.OrderId,
                    (orderEmployee, payment) => new { orderEmployee.order, orderEmployee.employee, payment })
                .GroupBy(x => new { x.employee.EmployeeId, x.employee.FirstName, x.employee.LastName })
                .Select(g => new EmployeeProductivityDto
                {
                    EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                    OrderCount = g.Count(),
                    TotalAmount = Math.Round(g.Sum(x => x.payment.Amount), 2)
                })
                .ToList();

            return productivityData;

        }

        private void GenerateEmployeeProductivityChart(List<EmployeeProductivityDto> productivityData)
        {
            var productivityChart = new CartesianChart();

            productivityChart.AxisY.Add(new Axis
            {
                Title = "Ilość zrealizowanych zamówień",
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = productivityData.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
            });

            productivityChart.AxisY.Add(new Axis
            {
                Title = "Suma kwot zamówień",
                Position = AxisPosition.RightTop,
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                },
                MinValue = productivityData.Select(p => p.TotalAmount).DefaultIfEmpty(0).Min() * 0.8,
                ShowLabels = false
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

        #region Card to cash payment ratio

        private async Task<List<PaymentRatioDto>> GenerateCardToCashPaymentRatioData(DateTime starTime, DateTime endTime)
        {
            await using var dbContext = new AppDbContext();

            var paymentRatio = await dbContext.Orders
                .Where(order => order.OrderTime >= starTime && order.OrderTime <= endTime)
                .Join(dbContext.Payments, order => order.OrderId, payment => payment.OrderId,
                    (order, payment) => payment)
                .GroupBy(payment => payment.PaymentMethod)
                .Select(group => new PaymentRatioDto
                {
                    PaymentMethod = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return paymentRatio;
        }

        private void GeneratePaymentMethodChart(List<PaymentRatioDto> paymentRatio)
        {
            var pieChart = new PieChart();

            var cardPayments = paymentRatio.FirstOrDefault(p => p.PaymentMethod == "Karta debetowa")?.Count ?? 0;
            var cashPayments = paymentRatio.FirstOrDefault(p => p.PaymentMethod == "Gotówka")?.Count ?? 0;

            pieChart.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Karta Debetowa",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(cardPayments) },
                    DataLabels = true,
                },
                new PieSeries
                {
                    Title = "Gotówka",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(cashPayments) },
                    DataLabels = true,
                }
            };

            liveChart.Children.Add(pieChart);
        }

        #endregion
    }
}