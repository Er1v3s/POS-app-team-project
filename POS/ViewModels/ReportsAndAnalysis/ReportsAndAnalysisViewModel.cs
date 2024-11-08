using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportsAndAnalysisViewModel : ViewModelBase
    {
        private int selectedReportIndex;
        private DateTime startDate = DateTime.Now.AddMonths(-1);
        private DateTime endDate = DateTime.Now;

        private SeriesCollection seriesCollection;
        private List<string> labels;

        public Func<double, string>? Values { get; set; }

        public List<string>? Labels
        {
            get => labels;
            set
            {
                SetField(ref labels, value);
                OnPropertyChanged(nameof(labels));
            }
        }

        public int SelectedReportIndex
        {
            get => selectedReportIndex;
            set => SetField(ref selectedReportIndex, value);
        }

        public DateTime StartDate
        {
            get => startDate;
            set => SetField(ref startDate, value);
        }

        public DateTime EndDate
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

        private readonly IReportGenerator<ProductSalesDto> _saleReportGenerator;
        private readonly IReportGenerator<RevenueReportDto> _revenueReportGenerator;
        private readonly IReportGenerator<OrderReportDto> _numberOfOrdersReportGenerator;
        private readonly IReportGenerator<EmployeeProductivityDto> _employeeProductivityReportGenerator;
        private readonly IReportGenerator<PaymentRatioDto> _paymentRatioReportGenerator;

        private readonly IChartGenerator<ProductSalesDto> _salesReportChartGenerator;
        private readonly IChartGenerator<RevenueReportDto> _revenueReportChartGenerator;
        private readonly IChartGenerator<OrderReportDto> _numberOfOrdersReportChartGenerator;
        private readonly IChartGenerator<EmployeeProductivityDto> _employeeProductivityReportChartGenerator;
        private readonly IChartGenerator<PaymentRatioDto> _paymentRatioReportChartGenerator;

        public ReportsAndAnalysisViewModel(
            IReportGenerator<ProductSalesDto> saleReportGenerator, 
            IReportGenerator<RevenueReportDto> revenueReportGenerator, 
            IReportGenerator<OrderReportDto> numberOfOrdersReportGenerator,
            IReportGenerator<EmployeeProductivityDto> employeeProductivityReportGenerator,
            IReportGenerator<PaymentRatioDto> paymentRatioReportGenerator,

            IChartGenerator<ProductSalesDto> salesReportChartGenerator,
            IChartGenerator<RevenueReportDto> revenueReportChartGenerator,
            IChartGenerator<OrderReportDto> numberOfOrdersReportChartGenerator,
            IChartGenerator<EmployeeProductivityDto> employeeProductivityReportChartGenerator,
            IChartGenerator<PaymentRatioDto> paymentRatioReportChartGenerator
            )
        {
            GenerateReportCommand = new RelayCommand(async _ => await GenerateReport());

            _saleReportGenerator = saleReportGenerator;
            _revenueReportGenerator = revenueReportGenerator;
            _numberOfOrdersReportGenerator = numberOfOrdersReportGenerator;
            _employeeProductivityReportGenerator = employeeProductivityReportGenerator;
            _paymentRatioReportGenerator = paymentRatioReportGenerator;

            _salesReportChartGenerator = salesReportChartGenerator;
            _revenueReportChartGenerator = revenueReportChartGenerator;
            _numberOfOrdersReportChartGenerator = numberOfOrdersReportChartGenerator;
            _employeeProductivityReportChartGenerator = employeeProductivityReportChartGenerator;
            _paymentRatioReportChartGenerator = paymentRatioReportChartGenerator;

            seriesCollection = new SeriesCollection();

            _ = GenerateDefaultChart();
        }

        private async Task GenerateReport()
        {
            var inputValidator = new InputValidator();
            var validationResult = inputValidator.ValidateInputs(selectedReportIndex, startDate, endDate);

            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.ErrorMessage);
                return;
            }

            seriesCollection.Clear();

            switch (selectedReportIndex)
            {
                case 0:
                    var productSalesData = await _saleReportGenerator.GenerateData(startDate, endDate);
                    _salesReportChartGenerator.GenerateChart(productSalesData, seriesCollection, out labels);
                    break;
                case 1:
                    var revenueDailyData = await _revenueReportGenerator.GenerateData(startDate, endDate, "day");
                    _revenueReportChartGenerator.GenerateChart(revenueDailyData, seriesCollection, out labels, r => r.Date.ToString("yyyy-MM-dd"));
                    break;
                case 2:
                    var revenueWeeklyData = await _revenueReportGenerator.GenerateData(startDate, endDate, "week");
                    _revenueReportChartGenerator.GenerateChart(revenueWeeklyData, seriesCollection, out labels, r => r.DayOfWeek.ToString());
                    break;
                case 3:
                    var revenueMonthlyData = await _revenueReportGenerator.GenerateData(startDate, endDate, "month");
                    _revenueReportChartGenerator.GenerateChart(revenueMonthlyData, seriesCollection, out labels, r => r.Date.ToString("yyyy-MM"));
                    break;
                case 4:
                    var revenueYearlyData = await _revenueReportGenerator.GenerateData(startDate, endDate, "year");
                    _revenueReportChartGenerator.GenerateChart(revenueYearlyData, seriesCollection, out labels, r => r.Date.ToString("yyyy"));
                    break;
                case 5:
                    var orderReportsByDays = await _numberOfOrdersReportGenerator.GenerateData(startDate, endDate, "day");
                    _numberOfOrdersReportChartGenerator.GenerateChart(orderReportsByDays, seriesCollection, out labels, o => o.Date.ToString("yyyy-MM-dd"));
                    break;
                case 6:
                    var orderReportsByDayOfWeeks = await _numberOfOrdersReportGenerator.GenerateData(startDate, endDate, "week");
                    _numberOfOrdersReportChartGenerator.GenerateChart(orderReportsByDayOfWeeks, seriesCollection, out labels, o => o.DayOfWeek.ToString());
                    break;
                case 7:
                    var orderReportsByMonths = await _numberOfOrdersReportGenerator.GenerateData(startDate, endDate, "month");
                    _numberOfOrdersReportChartGenerator.GenerateChart(orderReportsByMonths, seriesCollection, out labels, o => o.Date.ToString("yyyy-MM"));
                    break;
                case 8:
                    var orderReportsByYears = await _numberOfOrdersReportGenerator.GenerateData(startDate, endDate, "year");
                    _numberOfOrdersReportChartGenerator.GenerateChart(orderReportsByYears, seriesCollection, out labels, o => o.Date.ToString("yyyy"));
                    break;
                case 9:
                    var employeeProductivityData = await _employeeProductivityReportGenerator.GenerateData(startDate, endDate);
                    _employeeProductivityReportChartGenerator.GenerateChart(employeeProductivityData, seriesCollection, out labels);
                    break;
                case 10:
                    var paymentMethodData = await _paymentRatioReportGenerator.GenerateData(startDate, endDate);
                    _paymentRatioReportChartGenerator.GenerateChart(paymentMethodData, seriesCollection, out labels);
                    break;
            }

            OnPropertyChanged(nameof(labels));
        }

        private async Task GenerateDefaultChart()
        {
            var productSalesData = await _saleReportGenerator.GenerateData(DateTime.Now.AddMonths(-1), DateTime.Now);
            _salesReportChartGenerator.GenerateChart(productSalesData, seriesCollection, out labels);
        }
    }
}