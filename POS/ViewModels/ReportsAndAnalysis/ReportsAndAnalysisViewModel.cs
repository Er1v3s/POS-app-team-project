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
        private string[]? labels;
        public Func<double, string>? Values { get; set; }
        public string[]? Labels
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
                    MessageBox.Show($"Wykres {SelectedReportIndex} powinien być już widoczny start:{StartDate.Value} koniec:{EndDate.Value}");
                    break;
            }
        }

        private bool ValidateInputs()
        {
            return SelectedReportIndex >= 0 && StartDate.HasValue && EndDate.HasValue && StartDate <= EndDate;
        }



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

            Labels = productSales.Select(p => p.ProductName).ToArray();
            Values = value => value.ToString("N");
        }
    }
}
