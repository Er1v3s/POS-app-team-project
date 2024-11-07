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
using POS.Models.Reports;

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
                    GenerateRevenueChart(revenueDailyData, r => r.Date.ToString("yyyy-MM-dd"));
                    break;
                case 2:
                    var revenueWeeklyData = await GenerateRevenueReport(StartDate.Value, EndDate.Value, "week");
                    GenerateRevenueChart(revenueWeeklyData, r => r.DayOfWeek.ToString());
                    break;
                case 3:
                    var revenueMonthlyData = await GenerateRevenueReport(StartDate.Value, EndDate.Value, "month");
                    GenerateRevenueChart(revenueMonthlyData, r => r.Date.ToString("yyyy-MM"));
                    break;
                case 4:
                    var revenueYearlyData = await GenerateRevenueReport(startDate.Value, EndDate.Value, "year");
                    GenerateRevenueChart(revenueYearlyData, r => r.Date.ToString("yyyy"));
                    break;
                case 5:
                    var orderReportsByDays = await GenerateNumberOfOrders(StartDate.Value, EndDate.Value, "day");
                    GenerateNumberOfOrdersChart(orderReportsByDays, o => o.Date.ToString("yyyy-MM-dd"));
                    break;
                case 6:
                    var orderReportsByDayOfWeeks = await GenerateNumberOfOrders(StartDate.Value, EndDate.Value, "week");
                    GenerateNumberOfOrdersChart(orderReportsByDayOfWeeks, o => o.DayOfWeek.ToString());
                    break;
                case 7:
                    var orderReportsByMonths = await GenerateNumberOfOrders(StartDate.Value, EndDate.Value, "month");
                    GenerateNumberOfOrdersChart(orderReportsByMonths, o => o.Date.ToString("yyyy-MM"));
                    break;
                case 8:
                    var orderReportsByYears = await GenerateNumberOfOrders(startDate.Value, EndDate.Value, "year");
                    GenerateNumberOfOrdersChart(orderReportsByYears, o => o.Date.ToString("yyyy"));
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
                    (order, payment) => new { order.OrderTime, payment.Amount })
                .AsEnumerable();

            switch (groupBy)
            {
                case "day":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Date)
                        .Select(g => new RevenueReportDto
                        {
                            Date = g.Key,
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;
                case "week":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.DayOfWeek)
                        .Select(g => new RevenueReportDto
                        {
                            DayOfWeek = g.Key,
                            TotalRevenue = (float)g.Sum(x => x.Amount),
                        })
                        .OrderBy(order => order.DayOfWeek)
                        .AsQueryable();
                    break;
                case "month":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;
                case "year":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Year)
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key, 1, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            return groupedQuery.OrderBy(revenue => revenue.Date).ToList();
        }

        private void GenerateRevenueChart(List<RevenueReportDto> revenueReport, Func<RevenueReportDto, string> labelSelector)
        {
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Przychód",
                Values = new ChartValues<float>(revenueReport.Select(p => p.TotalRevenue)),
                LabelPoint = point => point.Y.ToString("C"),
                DataLabels = true,
            });

            Labels = revenueReport.Select(labelSelector).ToList();
        }

        #endregion

        #region NumberOfOrdersReports

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
                        });
                    break;
                case "week":
                    ordersReport = orders.GroupBy(order => order.DayOfWeek)
                        .Select(group => new OrderReportDto
                        {
                            DayOfWeek = group.Key,
                            OrderCount = group.Count()
                        })
                        .OrderBy(order => order.DayOfWeek);
                    break;
                case "month":
                    ordersReport = orders
                        .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                            OrderCount = group.Count()
                        });
                    break;

                case "year":
                    ordersReport = orders
                        .GroupBy(order => order.OrderTime.Year)
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key, 1, 1),
                            OrderCount = group.Count()
                        });
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            return ordersReport.OrderBy(order => order.Date).ToList();
        }


        private void GenerateNumberOfOrdersChart(List<OrderReportDto> ordersReport, Func<OrderReportDto, string> labelSelector)
        {
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Liczba zamówień",
                Values = new ChartValues<int>(ordersReport.Select(o => o.OrderCount)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            Labels = ordersReport.Select(labelSelector).ToList();
        }

        #endregion
    }
}
