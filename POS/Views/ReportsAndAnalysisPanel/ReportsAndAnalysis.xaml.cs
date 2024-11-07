using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis;
using Separator = LiveCharts.Wpf.Separator;

namespace POS.Views.ReportsAndAnalysisPanel
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : Page
    {

        public ReportsAndAnalysis()
        {
            InitializeComponent();
            DataContext = new ReportsAndAnalysisViewModel();
        }

        //private async void GenerateReport_ButtonClick(object sender, RoutedEventArgs e)
        //{
        //    int selectedReportIndex = reportTypeComboBox.SelectedIndex;
        //    DateTime startDate = datePickerFrom.SelectedDate.GetValueOrDefault();
        //    DateTime endDate = datePickerTo.SelectedDate.GetValueOrDefault().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

        //    if (ValidateInputs(selectedReportIndex, startDate, endDate))
        //    {
        //        liveChart.Children.Clear();
        //        await GenerateReport(selectedReportIndex, startDate, endDate);
        //    }
        //}

        //private bool ValidateInputs(int selectedReportIndex, DateTime startDate, DateTime endDate)
        //{
        //    if (selectedReportIndex < 0)
        //    {
        //        MessageBox.Show("Nie wybrano typu raportu");
        //        return false;
        //    }

        //    if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
        //    {
        //        MessageBox.Show("Nie wybrano zakresu czasu");
        //        return false;
        //    }

        //    if (endDate < startDate)
        //    {
        //        MessageBox.Show("Data 'Do' nie może być wcześniejsza niż data 'Od'");
        //        return false;
        //    }

        //    return true;
        //}

        //private async Task GenerateReport(int reportIndex, DateTime startDate, DateTime endDate)
        //{
        //    switch (reportIndex)
        //    {
        //        case 0:
        //            List<ProductSalesDto> productSalesData = await GenerateSalesReport(startDate, endDate);
        //            GenerateSalesReportChart(productSalesData);
        //            break;
        //        case 1:
        //            List<RevenueReportDto> revenueDailyData = await GenerateRevenueReport(startDate, endDate, "Daily");
        //            GenerateRevenueChart(revenueDailyData, "Przychód", "Data", p => p.Date.ToString("yyyy-MM-dd"));
        //            break;
        //        case 2:
        //            List<RevenueReportDto> revenueMonthlyData = await GenerateRevenueReport(startDate, endDate, "Monthly");
        //            GenerateRevenueChart(revenueMonthlyData, "Przychód", "Miesiąc", p => $"{p.Month:00}-{p.Year}");

        //            //await GenerateRevenueReportWithForecast(startDate, endDate, "Monthly");
        //            break;
        //        case 3:
        //            List<RevenueReportDto> revenueYearlyData = await GenerateRevenueReport(startDate, endDate, "Yearly");
        //            GenerateRevenueChart(revenueYearlyData, "Przychód", "Rok", p => p.Year.ToString());
        //            break;
        //        case 4:
        //            List<OrderReportDto> orderReportsOnDays = await GenerateNumberOfOrdersOnDays(startDate, endDate);
        //            GenerateOrdersChartForDays(orderReportsOnDays);
        //            break;
        //        case 5:
        //            List<OrderReportDto> orderReportsOnMonths = await GenerateNumberOfOrdersByMonths(startDate, endDate);
        //            GenerateOrdersChartForMonths(orderReportsOnMonths);
        //            break;
        //        case 6:
        //            List<OrderReportDto> orderReportsOnYears = await GenerateNumberOfOrdersByYears(startDate, endDate);
        //            GenerateOrdersChartForYears(orderReportsOnYears);
        //            break;
        //        case 7:
        //            List<OrderReportDto> orderReportsOnSpecificDay = await GenerateNumberOfOrdersOnSpecificDays(startDate, endDate);
        //            GenerateOrdersChart(orderReportsOnSpecificDay);
        //            break;
        //        case 8:
        //            List<EmployeeProductivityDto> employeeProductivityData =
        //                await GenerateEmployeeProductivityData(startDate, endDate);
        //            GenerateEmployeeProductivityChart(employeeProductivityData);
        //            break;
        //        case 9:
        //            List<PaymentRatioDto> paymentRatio = await GenerateCardToCashPaymentRatioData(startDate, endDate);
        //            GeneratePaymentMethodChart(paymentRatio);
        //            break;
        //        default:
        //            MessageBox.Show("Wybrany raport nie jest dostępny.");
        //            break;
        //    }
        //}


        //#region Sales raport

        //private async Task<List<ProductSalesDto>> GenerateSalesReport(DateTime startDate, DateTime endDate)
        //{
        //    await using var dbContext = new AppDbContext();

        //    var productSales = await dbContext.OrderItems
        //        .Where(orderItem => dbContext.Orders
        //            .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //            .Select(order => order.OrderId)
        //            .Contains(orderItem.OrderId))
        //        .GroupBy(orderItem => orderItem.ProductId)
        //        .Select(groupedItems => new ProductSalesDto
        //        {
        //            ProductName = dbContext.Products
        //                .Where(product => product.ProductId == groupedItems.Key)
        //                .Select(product => product.ProductName)
        //                .FirstOrDefault(),
        //            Quantity = groupedItems.Sum(item => item.Quantity)
        //        })
        //        .ToListAsync();

        //    return productSales;
        //}

        //private void GenerateSalesReportChart(List<ProductSalesDto> productSales)
        //{
        //    var salesProductsChart = new CartesianChart();

        //    salesProductsChart.AxisY.Add(new Axis
        //    {
        //        Title = "Ilość sprzedanych produktów",
        //        Separator = new Separator
        //        {
        //            Step = 1,
        //            IsEnabled = true
        //        },
        //        MinValue = (double)(productSales.Min(p => p.Quantity) * 0.8m),
        //        ShowLabels = false
        //    });

        //    salesProductsChart.Series = new SeriesCollection
        //    {
        //        new ColumnSeries
        //        {
        //            Title = "Ilość sprzedanych produktów: ",
        //            Values = new ChartValues<int>(productSales.Select(p => (p.Quantity))),
        //            DataLabels = true,
        //        }
        //    };

        //    salesProductsChart.AxisX.Add(new Axis
        //    {
        //        Title = "Produkt",
        //        Labels = productSales.Select(p => p.ProductName).ToList(),
        //        IsEnabled = true
        //    });

        //    liveChart.Children.Add(salesProductsChart);
        //}

        //#endregion



















        //private async Task GenerateRevenueReportWithForecast(DateTime startDate, DateTime endDate, string groupBy)
        //{
        //    var revenueReport = await GenerateRevenueReport(startDate, endDate, groupBy);

        //    var salesPredictionService = new SalesPredictionService();
        //    var model = salesPredictionService.TrainModel(revenueReport);
        //    var forecast = salesPredictionService.Predict(model, revenueReport);

        //    DisplayForecastChart(forecast);
        //}

        //private void DisplayForecastChart(RevenuePrediction forecast)
        //{
        //    var forecastChart = new CartesianChart();

        //    forecastChart.Series = new SeriesCollection
        //    {
        //        new ColumnSeries
        //        {
        //            Title = "Prognozowana sprzedaż",
        //            Values = new ChartValues<float>(forecast.ForecastedRevenue)
        //        }
        //    };

        //    forecastChart.AxisX.Add(new Axis
        //    {
        //        Title = "Miesiące",
        //        Labels = Enumerable.Range(1, forecast.ForecastedRevenue.Length)
        //            .Select(i => $"Miesiąc {i}").ToArray(),
        //    });

        //    forecastChart.AxisY.Add(new Axis
        //    {
        //        Title = "Sprzedaż",
        //        Separator = new Separator
        //        {
        //            Step = 1,
        //            IsEnabled = true
        //        },
        //        MinValue = forecast.ForecastedRevenue.Min() * 0.9f,
        //        MaxValue = forecast.ForecastedRevenue.Max() * 1.1f,
        //        ShowLabels = false
        //    });

        //    liveChart.Children.Add(forecastChart);
        //}















        //    #region RevenueReports

        //    private async Task<List<RevenueReportDto>> GenerateRevenueReport(DateTime startDate, DateTime endDate,
        //        string groupBy)
        //    {
        //        await using var dbContext = new AppDbContext();

        //        var revenueReportQuery = dbContext.Orders
        //            .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //            .Join(dbContext.Payments,
        //                order => order.OrderId,
        //                payment => payment.OrderId,
        //                (order, payment) => new { order.OrderTime, payment.Amount });

        //        IQueryable<RevenueReportDto> groupedQuery;

        //        switch (groupBy)
        //        {
        //            case "Daily":
        //                groupedQuery = revenueReportQuery
        //                    .GroupBy(x => x.OrderTime.Date)
        //                    .Select(g => new RevenueReportDto
        //                    {
        //                        Date = g.Key,
        //                        TotalRevenue = (float)g.Sum(x => x.Amount)
        //                    });
        //                break;

        //            case "Monthly":
        //                groupedQuery = revenueReportQuery
        //                    .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
        //                    .Select(g => new RevenueReportDto
        //                    {
        //                        Year = g.Key.Year,
        //                        Month = g.Key.Month,
        //                        TotalRevenue = (float)g.Sum(x => x.Amount)
        //                    });
        //                break;

        //            case "Yearly":
        //                groupedQuery = revenueReportQuery
        //                    .GroupBy(x => x.OrderTime.Year)
        //                    .Select(g => new RevenueReportDto
        //                    {
        //                        Year = g.Key,
        //                        TotalRevenue = (float)g.Sum(x => x.Amount)
        //                    });
        //                break;

        //            default:
        //                throw new ArgumentException("Invalid groupBy value");
        //        }

        //        return groupedQuery
        //            .AsEnumerable() // Switch to client-side evaluation
        //            .OrderBy(r => r.Year)
        //            .ThenBy(r => r.Month)
        //            .ToList();
        //    }

        //    private void GenerateRevenueChart(List<RevenueReportDto> revenueReport, string title, string xAxisTitle,
        //        Func<RevenueReportDto, string> labelSelector)
        //    {
        //        var revenueChart = new CartesianChart();

        //        revenueChart.AxisY.Add(new Axis
        //        {
        //            Title = "Przychód",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = (revenueReport.Min(p => p.TotalRevenue) * 0.8f),
        //            ShowLabels = false
        //        });

        //        revenueChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = title,
        //                Values = new ChartValues<float>(revenueReport.Select(p => p.TotalRevenue)),
        //                DataLabels = true,
        //            }
        //        };

        //        revenueChart.AxisX.Add(new Axis
        //        {
        //            Title = xAxisTitle,
        //            Labels = revenueReport.Select(labelSelector).ToList(),
        //            IsEnabled = true
        //        });

        //        liveChart.Children.Add(revenueChart);
        //    }


        //    #endregion

        //    #region NumberOfOrdersReports

        //    #region NumberOfOrdersOnSpecificDays

        //    private async Task<List<OrderReportDto>> GenerateNumberOfOrdersOnSpecificDays(DateTime startDate, DateTime endDate)
        //    {
        //        await using (var dbContext = new AppDbContext())
        //        {
        //            var ordersReport = await dbContext.Orders
        //                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //                .GroupBy(order => order.DayOfWeek)
        //                .Select(group => new OrderReportDto
        //                {
        //                    DayOfWeek = group.Key,
        //                    OrderCount = group.Count()
        //                })
        //                .OrderBy(order => order.DayOfWeek)
        //                .ToListAsync();

        //            return ordersReport;
        //        }
        //    }

        //    private void GenerateOrdersChart(List<OrderReportDto> ordersReport)
        //    {
        //        var ordersChart = new CartesianChart();

        //        ordersChart.AxisY.Add(new Axis
        //        {
        //            Title = "Liczba zamówień",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

        //        ordersChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = "Zamówienia",
        //                Values = chartValues,
        //                DataLabels = true,
        //            }
        //        };

        //        ordersChart.AxisX.Add(new Axis
        //        {
        //            Title = "Dzień tygodnia",
        //            Labels = ordersReport.Select(o => o.DayOfWeek.ToString()).ToList(),
        //            IsEnabled = true
        //        });

        //        liveChart.Children.Add(ordersChart);
        //    }


        //    #endregion

        //    #region NumberOfOrdersOnDays

        //    private async Task<List<OrderReportDto>> GenerateNumberOfOrdersOnDays(DateTime startDate, DateTime endDate)
        //    {
        //        await using (var dbContext = new AppDbContext())
        //        {
        //            var ordersReport = dbContext.Orders
        //                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //                .GroupBy(order => order.OrderTime.Date)
        //                .Select(group => new OrderReportDto
        //                {
        //                    Date = group.Key,
        //                    OrderCount = group.Count()
        //                })
        //                .AsEnumerable()
        //                .OrderBy(order => order.Date.DayOfWeek)
        //                .ToList();

        //            return ordersReport;
        //        }
        //    }

        //    private void GenerateOrdersChartForDays(List<OrderReportDto> ordersReport)
        //    {
        //        var ordersChart = new CartesianChart();

        //        ordersChart.AxisY.Add(new Axis
        //        {
        //            Title = "Liczba zamówień",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

        //        ordersChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = "Zamówienia",
        //                Values = chartValues,
        //                DataLabels = true,
        //            }
        //        };

        //        ordersChart.AxisX.Add(new Axis
        //        {
        //            Title = "Data",
        //            Labels = ordersReport.Select(o => o.Date.ToString("yyyy-MM-dd")).ToList(),
        //            IsEnabled = true
        //        });

        //        liveChart.Children.Add(ordersChart);
        //    }

        //    #endregion

        //    #region NumberOfOrdersByMonths

        //    private async Task<List<OrderReportDto>> GenerateNumberOfOrdersByMonths(DateTime startDate, DateTime endDate)
        //    {
        //        await using (var dbContext = new AppDbContext())
        //        {
        //            var ordersReport = dbContext.Orders
        //                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //                .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
        //                .Select(group => new OrderReportDto
        //                {
        //                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
        //                    OrderCount = group.Count()
        //                })
        //                .AsEnumerable()
        //                .OrderBy(order => order.Date.DayOfWeek)
        //                .ToList();

        //            return ordersReport;
        //        }
        //    }


        //    private void GenerateOrdersChartForMonths(List<OrderReportDto> ordersReport)
        //    {
        //        var ordersChart = new CartesianChart();

        //        ordersChart.AxisY.Add(new Axis
        //        {
        //            Title = "Liczba zamówień",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

        //        ordersChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = "Zamówienia",
        //                Values = chartValues,
        //                DataLabels = true,
        //            }
        //        };

        //        ordersChart.AxisX.Add(new Axis
        //        {
        //            Title = "Miesiąc",
        //            Labels = ordersReport.Select(o => new DateTime(1, 1, (int)o.DayOfWeek + 1).ToString("MMMM yyyy"))
        //                .ToList(),
        //            IsEnabled = true
        //        });

        //        liveChart.Children.Add(ordersChart);
        //    }

        //    #endregion

        //    #region NumberOfOrdersByYears

        //    private async Task<List<OrderReportDto>> GenerateNumberOfOrdersByYears(DateTime startDate, DateTime endDate)
        //    {
        //        await using (var dbContext = new AppDbContext())
        //        {
        //            var ordersReport = dbContext.Orders
        //                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //                .GroupBy(order => order.OrderTime.Year)
        //                .AsEnumerable()
        //                .Select(group => new OrderReportDto
        //                {
        //                    Date = new DateTime(group.Key, 1, 1),
        //                    OrderCount = group.Count()
        //                })
        //                .OrderBy(order => order.Date.DayOfWeek)
        //                .ToList();

        //            return ordersReport;
        //        }
        //    }


        //    private void GenerateOrdersChartForYears(List<OrderReportDto> ordersReport)
        //    {
        //        var ordersChart = new CartesianChart();

        //        ordersChart.AxisY.Add(new Axis
        //        {
        //            Title = "Liczba zamówień",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = ordersReport.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        var chartValues = new ChartValues<int>(ordersReport.Select(o => o.OrderCount));

        //        ordersChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = "Zamówienia",
        //                Values = chartValues,
        //                DataLabels = true,
        //            }
        //        };

        //        ordersChart.AxisX.Add(new Axis
        //        {
        //            Title = "Rok",
        //            Labels = ordersReport.Select(o => o.Date.Year.ToString()).ToList(),
        //            IsEnabled = true
        //        });

        //        liveChart.Children.Add(ordersChart);
        //    }

        //    #endregion

        //    #endregion

        //    #region Employee productivity raport

        //    private async Task<List<EmployeeProductivityDto>> GenerateEmployeeProductivityData(DateTime startDate,
        //        DateTime endDate)
        //    {
        //        await using var dbContext = new AppDbContext();

        //        var productivityData = dbContext.Orders
        //            .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
        //            .Join(dbContext.Employees, order => order.EmployeeId, employee => employee.EmployeeId,
        //                (order, employee) => new { order, employee })
        //            .Join(dbContext.Payments, orderEmployee => orderEmployee.order.OrderId, payment => payment.OrderId,
        //                (orderEmployee, payment) => new { orderEmployee.order, orderEmployee.employee, payment })
        //            .GroupBy(x => new { x.employee.EmployeeId, x.employee.FirstName, x.employee.LastName })
        //            .Select(g => new EmployeeProductivityDto
        //            {
        //                EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
        //                OrderCount = g.Count(),
        //                TotalAmount = Math.Round(g.Sum(x => x.payment.Amount), 2)
        //            })
        //            .ToList();

        //        return productivityData;

        //    }

        //    private void GenerateEmployeeProductivityChart(List<EmployeeProductivityDto> productivityData)
        //    {
        //        var productivityChart = new CartesianChart();

        //        productivityChart.AxisY.Add(new Axis
        //        {
        //            Title = "Ilość zrealizowanych zamówień",
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = productivityData.Select(p => p.OrderCount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        productivityChart.AxisY.Add(new Axis
        //        {
        //            Title = "Suma kwot zamówień",
        //            Position = AxisPosition.RightTop,
        //            Separator = new Separator
        //            {
        //                Step = 1,
        //                IsEnabled = true
        //            },
        //            MinValue = productivityData.Select(p => p.TotalAmount).DefaultIfEmpty(0).Min() * 0.8,
        //            ShowLabels = false
        //        });

        //        productivityChart.Series = new SeriesCollection
        //        {
        //            new ColumnSeries
        //            {
        //                Title = "Ilość zrealizowanych zamówień: ",
        //                Values = new ChartValues<int>(productivityData.Select(p => p.OrderCount)),
        //                DataLabels = true,
        //            },
        //            new ColumnSeries
        //            {
        //                Title = "Suma kwot zamówień: ",
        //                Values = new ChartValues<double>(productivityData.Select(p => p.TotalAmount)),
        //                DataLabels = true,
        //                ScalesYAt = 1 // Ta seria będzie korzystać z drugiej osi Y
        //            }
        //        };

        //        productivityChart.AxisX.Add(new Axis
        //        {
        //            Title = "Pracownik",
        //            Labels = productivityData.Select(p => p.EmployeeName).ToList()
        //        });

        //        liveChart.Children.Add(productivityChart);
        //    }



        //    #endregion

        //    #region Card to cash payment ratio

        //    private async Task<List<PaymentRatioDto>> GenerateCardToCashPaymentRatioData(DateTime starTime, DateTime endTime)
        //    {
        //        await using var dbContext = new AppDbContext();

        //        var paymentRatio = await dbContext.Orders
        //            .Where(order => order.OrderTime >= starTime && order.OrderTime <= endTime)
        //            .Join(dbContext.Payments, order => order.OrderId, payment => payment.OrderId,
        //                (order, payment) => payment)
        //            .GroupBy(payment => payment.PaymentMethod)
        //            .Select(group => new PaymentRatioDto
        //            {
        //                PaymentMethod = group.Key,
        //                Count = group.Count()
        //            })
        //            .ToListAsync();

        //        return paymentRatio;
        //    }

        //    private void GeneratePaymentMethodChart(List<PaymentRatioDto> paymentRatio)
        //    {
        //        var pieChart = new PieChart();

        //        var cardPayments = paymentRatio.FirstOrDefault(p => p.PaymentMethod == "Karta debetowa")?.Count ?? 0;
        //        var cashPayments = paymentRatio.FirstOrDefault(p => p.PaymentMethod == "Gotówka")?.Count ?? 0;

        //        pieChart.Series = new SeriesCollection
        //        {
        //            new PieSeries
        //            {
        //                Title = "Karta Debetowa",
        //                Values = new ChartValues<ObservableValue> { new ObservableValue(cardPayments) },
        //                DataLabels = true,
        //            },
        //            new PieSeries
        //            {
        //                Title = "Gotówka",
        //                Values = new ChartValues<ObservableValue> { new ObservableValue(cashPayments) },
        //                DataLabels = true,
        //            }
        //        };

        //        liveChart.Children.Add(pieChart);
        //    }

        //    #endregion
    }
}