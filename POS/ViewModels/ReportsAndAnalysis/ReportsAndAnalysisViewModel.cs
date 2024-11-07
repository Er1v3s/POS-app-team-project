using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POS.Models.Reports;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportsAndAnalysisViewModel : ViewModelBase
    {
        private int selectedReportIndex;
        private DateTime? startDate;
        private DateTime? endDate;
        private SeriesCollection? seriesCollection;
        private List<string>? labels;

        public Func<double, string>? Values { get; set; }

        public List<string>? Labels
        {
            get => labels;
            set
            {
                labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        public int SelectedReportIndex
        {
            get => selectedReportIndex;
            set => SetField(ref selectedReportIndex, value);
        }

        public DateTime? StartDate
        {
            get => startDate;
            set => SetField(ref startDate, value);
        }

        public DateTime? EndDate
        {
            get => endDate;
            set => SetField(ref endDate, value);
        }

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set => SetField(ref seriesCollection, value);
        }

        public ICommand GenerateReportCommand { get; }

        public ReportsAndAnalysisViewModel()
        {
            GenerateReportCommand = new RelayCommand(async _ => await GenerateReport());
            SeriesCollection = new SeriesCollection();
        }

        private async Task GenerateReport()
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Formularz wypełniony nieprawidłowo");
                return;
            }

            SeriesCollection.Clear();

            switch (SelectedReportIndex)
            {
                case 0:
                    var productSalesData = await GenerateSalesReport(StartDate.Value, EndDate.Value);
                    GenerateSalesReportChart(productSalesData);
                    break;
                case 1:
                    var revenueDailyData = await GenerateRevenueReport(StartDate.Value, EndDate.Value, "day");
                    GenerateRevenueChart(revenueDailyData, "yyyy-MM-dd");
                    break;
                case 2:
                    var revenueMonthlyData = await GenerateRevenueReport(StartDate.Value, EndDate.Value, "month");
                    GenerateRevenueChart(revenueMonthlyData, "yyyy-MM");
                    break;
                case 3:
                    var revenueYearlyData = await GenerateRevenueReport(startDate.Value, EndDate.Value, "year");
                    GenerateRevenueChart(revenueYearlyData,  "yyyy");
                    break;
                case 4:
                    var orderReportsByDays = await GenerateNumberOfOrders(StartDate.Value, EndDate.Value, "day");
                    GenerateNumberOfOrdersChart(orderReportsByDays, "yyyy-MM-dd");
                    break;
                case 5:
                    var orderReportsByMonths = await GenerateNumberOfOrders(StartDate.Value, EndDate.Value, "month");
                    GenerateNumberOfOrdersChart(orderReportsByMonths, "yyyy-MM");
                    break;
                case 6:
                    var orderReportsByYears = await GenerateNumberOfOrders(startDate.Value, EndDate.Value, "year");
                    GenerateNumberOfOrdersChart(orderReportsByYears, "yyyy");
                    break;
            }
        }

        private bool ValidateInputs()
        {
            return SelectedReportIndex >= 0 && StartDate.HasValue && EndDate.HasValue && StartDate <= EndDate;
        }


        #region Sales Report

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
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Ilość sprzedanych produktów: ",
                Values = new ChartValues<int>(productSales.Select(p => (p.Quantity))),
                DataLabels = true,
            });

            Labels = productSales.Select(p => p.ProductName).ToList();
            Values = value => value.ToString("N");
        }

        #endregion


        #region RevenueReports

        private async Task<List<RevenueReportDto>> GenerateRevenueReport(DateTime startDate, DateTime endDate, string groupBy)
        {
            IQueryable<RevenueReportDto> groupedQuery;

            await using var dbContext = new AppDbContext();

            var revenueReportQuery = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new { order.OrderTime, payment.Amount });

            switch (groupBy)
            {
                case "day":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Date)
                        .Select(g => new RevenueReportDto
                        {
                            Date = g.Key,
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        });
                    break;

                case "month":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        });
                    break;

                case "year":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Year)
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key, 1, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        });
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            var revenueReport = await groupedQuery.ToListAsync();

            if (groupBy == "day")
            {
                revenueReport = AddMissingDates(revenueReport, startDate, endDate);
            }

            revenueReport = revenueReport.OrderBy(r => r.Date)
                .ThenBy(r => r.Month)
                .ThenBy(r => r.Year)
                .ToList();

            return revenueReport;
        }

        private List<RevenueReportDto> AddMissingDates(List<RevenueReportDto> revenueReport, DateTime startDate, DateTime endDate)
        {
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset).Date)
                .ToList();

            foreach (var date in allDates)
            {
                if (revenueReport.All(r => r.Date != date))
                {
                    revenueReport.Add(new RevenueReportDto { Date = date, TotalRevenue = 0 });
                }
            }

            return revenueReport;
        }

        private void GenerateRevenueChart(List<RevenueReportDto> revenueReport, string dateFormat)
        {
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Przychód",
                Values = new ChartValues<float>(revenueReport.Select(p => p.TotalRevenue)),
                LabelPoint = point => point.Y.ToString("C"),
                DataLabels = true,
            });

            Labels = revenueReport.Select(r => r.Date.ToString(dateFormat)).ToList();
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
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Liczba zamówień", // temp title
                Values = new ChartValues<int>(ordersReport.Select(o => o.OrderCount)),
                LabelPoint = point => point.Y.ToString("N"),
                DataLabels = true,
            });

            Labels = ordersReport.Select(o => o.DayOfWeek.ToString()).ToList();
        }


        #endregion

        private async Task<List<OrderReportDto>> GenerateNumberOfOrders(DateTime startDate, DateTime endDate, string groupBy)
        {
            await using var dbContext = new AppDbContext();

            var orders = await dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .ToListAsync();

            IEnumerable<OrderReportDto> ordersReport;

            switch (groupBy)
            {
                case "day":
                    ordersReport = orders
                        .GroupBy(order => order.OrderTime.Date)
                        .Select(group => new OrderReportDto
                        {
                            Date = group.Key,
                            OrderCount = group.Count()
                        })
                        .OrderBy(order => order.Date);
                    break;

                case "month":
                    ordersReport = orders
                        .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                            OrderCount = group.Count()
                        })
                        .OrderBy(order => order.Date);
                    break;

                case "year":
                    ordersReport = orders
                        .GroupBy(order => order.OrderTime.Year)
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key, 1, 1),
                            OrderCount = group.Count()
                        })
                        .OrderBy(order => order.Date);
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            return ordersReport.ToList();
        }


        private void GenerateNumberOfOrdersChart(List<OrderReportDto> ordersReport, string dateFormat)
        {
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Liczba zamówień",
                Values = new ChartValues<int>(ordersReport.Select(o => o.OrderCount)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            Labels = ordersReport.Select(o => o.Date.ToString(dateFormat)).ToList();
        }

        #endregion
    }
}
