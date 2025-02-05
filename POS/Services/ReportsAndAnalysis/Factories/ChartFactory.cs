using System;
using System.Collections.Generic;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.Services.ReportsAndAnalysis.Interfaces;
using ChartType = POS.Models.Reports.ChartType;
using SeriesCollection = LiveCharts.SeriesCollection;

namespace POS.Services.ReportsAndAnalysis.Factories
{
    public class ChartFactory : IChartsFactory
    {
        private SeriesCollection seriesCollection;
        private List<string> labels;

        private readonly IReportsFactory _reportFactory;
        private readonly IPredictionsFactory _predictionsFactory;

        private readonly Dictionary<int, Action> _reportChartGenerators;
        private readonly Dictionary<int, Action> _predictionChartGenerators;

        public ChartFactory(
            IReportsFactory reportFactory,
            IPredictionsFactory predictionsFactory,

            IChartGenerator<ProductSalesDto> salesReportChartGenerator,
            IChartGenerator<RevenueReportDto> revenueReportChartGenerator,
            IChartGenerator<OrderReportDto> numberOfOrdersReportChartGenerator,
            IChartGenerator<EmployeeProductivityDto> employeeProductivityReportChartGenerator,
            IChartGenerator<PaymentRatioDto> paymentRatioReportChartGenerator,

            IChartGenerator<ProductSalesPredictionDto> salesPredictionChartGenerator,
            IChartGenerator<RevenuePredictionDto> revenuePredictionChartGenerator,
            IChartGenerator<NumberOfOrdersPredictionDto> numberOfOrdersPredictionChartGenerator
            )
        {
            _reportFactory = reportFactory;
            _predictionsFactory = predictionsFactory;

            _reportChartGenerators = new Dictionary<int, Action>
            {
                { 0, () => GenerateReportChart(salesReportChartGenerator) },
                { 1, () => GenerateReportChart(salesReportChartGenerator) },
                { 2, () => GenerateReportChart(salesReportChartGenerator) },
                { 3, () => GenerateReportChart(salesReportChartGenerator) },
                { 4, () => GenerateReportChart(revenueReportChartGenerator,r => r.Date.ToString("yyyy-MM-dd")) },
                { 5, () => GenerateReportChart(revenueReportChartGenerator, r => r.DayOfWeek.ToString()) },
                { 6, () => GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 7, () => GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy")) },
                { 8, () => GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM-dd")) },
                { 9, () => GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.DayOfWeek.ToString()) },
                { 10, () => GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM")) },
                { 11, () => GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy")) },
                { 12, () => GenerateReportChart(employeeProductivityReportChartGenerator) },
                { 13, () => GenerateReportChart(paymentRatioReportChartGenerator) },
            };

            _predictionChartGenerators = new Dictionary<int, Action>
            {
                { 0, () => GeneratePredictionChart(salesPredictionChartGenerator) },
                { 1, () => GeneratePredictionChart(salesPredictionChartGenerator) },
                { 2, () => GeneratePredictionChart(salesPredictionChartGenerator) },
                { 3, () => GeneratePredictionChart(salesPredictionChartGenerator) },
                { 4, () => GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 5, () => GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 6, () => GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 7, () => GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy")) },
                { 8, () => GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 9, () => GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 10, () => GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 11, () => GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy")) },
            };
        }

        public void GenerateChart(int selectedReportIndex, SeriesCollection seriesCollection, ChartType chartType)
        {
            this.seriesCollection = seriesCollection;

            if (chartType == ChartType.Report)
                _reportChartGenerators[selectedReportIndex]();

            if (chartType == ChartType.Prediction)
                _predictionChartGenerators[selectedReportIndex]();
        }

        public List<string> GetUpdatedLabelsValues()
        {
            return labels;
        }

        private void GenerateReportChart<T>(IChartGenerator<T> chartGenerator,
                    Func<dynamic, string>? labelSelector = null)
        {
            var data = _reportFactory.GetReportData() as List<T>;

            chartGenerator.GenerateChart(data, seriesCollection, out labels, labelSelector);
        }

        private void GeneratePredictionChart<T>(IChartGenerator<T> chartGenerator,
            Func<dynamic, string>? labelSelector = null)
        {
            var data = _predictionsFactory.GetPredictionData() as List<T>;

            chartGenerator.GenerateChart(data, seriesCollection, out labels, labelSelector);
        }
    }
}
