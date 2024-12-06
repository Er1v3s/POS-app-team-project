using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using ChartType = POS.Models.Reports.ChartType;
using SeriesCollection = LiveCharts.SeriesCollection;

namespace POS.ViewModels.ReportsAndAnalysis.Factories
{
    public class ChartsFactory : IChartsFactory
    {
        private SeriesCollection seriesCollection;
        private List<string> labels;

        private readonly IReportsFactory _reportFactory;
        private readonly IPredictionsFactory _predictionsFactory;

        private readonly Dictionary<int, Func<Task>> _reportChartGenerators;
        private readonly Dictionary<int, Func<Task>> _predictionChartGenerators;

        public ChartsFactory(
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

            _reportChartGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReportChart(salesReportChartGenerator) },
                { 1, async () => await GenerateReportChart(salesReportChartGenerator) },
                { 2, async () => await GenerateReportChart(salesReportChartGenerator) },
                { 3, async () => await GenerateReportChart(salesReportChartGenerator) },
                { 4, async () => await GenerateReportChart(revenueReportChartGenerator,r => r.Date.ToString("yyyy-MM-dd")) },
                { 5, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.DayOfWeek.ToString()) },
                { 6, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 7, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy")) },
                { 8, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM-dd")) },
                { 9, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.DayOfWeek.ToString()) },
                { 10, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM")) },
                { 11, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy")) },
                { 12, async () => await GenerateReportChart(employeeProductivityReportChartGenerator) },
                { 13, async () => await GenerateReportChart(paymentRatioReportChartGenerator) },
            };

            _predictionChartGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GeneratePredictionChart(salesPredictionChartGenerator) },
                { 1, async () => await GeneratePredictionChart(salesPredictionChartGenerator) },
                { 2, async () => await GeneratePredictionChart(salesPredictionChartGenerator) },
                { 3, async () => await GeneratePredictionChart(salesPredictionChartGenerator) },
                { 4, async () => await GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 5, async () => await GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 6, async () => await GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 7, async () => await GeneratePredictionChart(revenuePredictionChartGenerator, r => r.Date.ToString("yyyy")) },
                { 8, async () => await GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 9, async () => await GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd")) },
                { 10, async () => await GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 11, async () => await GeneratePredictionChart(numberOfOrdersPredictionChartGenerator, r => r.Date.ToString("yyyy")) },
            };
        }

        public async Task GenerateChart(int selectedReportIndex, SeriesCollection seriesCollection, ChartType chartType)
        {
            this.seriesCollection = seriesCollection;

            if (chartType == ChartType.Report)
                await _reportChartGenerators[selectedReportIndex]();

            if (chartType == ChartType.Prediction)
                await _predictionChartGenerators[selectedReportIndex]();
        }

        public List<string> GetUpdatedLabelsValues()
        {
            return labels;
        }

        private async Task GenerateReportChart<T>(IChartGenerator<T> chartGenerator,
                    Func<dynamic, string>? labelSelector = null)
        {
            var data = _reportFactory.GetReportData() as IQueryable<T>;

            chartGenerator.GenerateChart(data, seriesCollection, out labels, labelSelector);
        }

        private async Task GeneratePredictionChart<T>(IChartGenerator<T> chartGenerator,
            Func<dynamic, string>? labelSelector = null)
        {
            var data = _predictionsFactory.GetPredictionData() as IQueryable<T>;

            chartGenerator.GenerateChart(data, seriesCollection, out labels, labelSelector);
        }
    }
}
