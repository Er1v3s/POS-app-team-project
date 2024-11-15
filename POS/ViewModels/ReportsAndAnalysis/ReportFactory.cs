using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.Predictions;
using SeriesCollection = LiveCharts.SeriesCollection;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportFactory : IReportFactory
    {
        private readonly Dictionary<int, Func<Task>> reportDataGenerators;
        private readonly Dictionary<int, Func<Task>> reportChartGenerators;

        private SeriesCollection seriesCollection;
        private DateTime startDate;
        private DateTime endDate;
        private List<string> labels;
        private object reportData;
        private readonly IChartGenerator<RevenuePredictionDto> predictionChartGenerator;

        private List<RevenuePredictionDto> revenuePredictions;

        public ReportFactory(
            IReportGenerator<ProductSalesDto> saleReportGenerator,
            IReportGenerator<RevenueReportDto> revenueReportGenerator,
            IReportGenerator<OrderReportDto> numberOfOrdersReportGenerator,
            IReportGenerator<EmployeeProductivityDto> employeeProductivityReportGenerator,
            IReportGenerator<PaymentRatioDto> paymentRatioReportGenerator,

            IChartGenerator<ProductSalesDto> salesReportChartGenerator,
            IChartGenerator<RevenueReportDto> revenueReportChartGenerator,
            IChartGenerator<OrderReportDto> numberOfOrdersReportChartGenerator,
            IChartGenerator<EmployeeProductivityDto> employeeProductivityReportChartGenerator,
            IChartGenerator<PaymentRatioDto> paymentRatioReportChartGenerator,
            IChartGenerator<RevenuePredictionDto> predictionChartGenerator)
        {
            this.predictionChartGenerator = predictionChartGenerator;

            reportDataGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReportData(saleReportGenerator) },
                { 1, async () => await GenerateReportData(revenueReportGenerator, "day")},
                { 2, async () => await GenerateReportData(revenueReportGenerator, "week") },
                { 3, async () => await GenerateReportData(revenueReportGenerator, "month") },
                { 4, async () => await GenerateReportData(revenueReportGenerator, "year") },
                { 5, async () => await GenerateReportData(numberOfOrdersReportGenerator, "day") },
                { 6, async () => await GenerateReportData(numberOfOrdersReportGenerator, "week") },
                { 7, async () => await GenerateReportData(numberOfOrdersReportGenerator, "month") },
                { 8, async () => await GenerateReportData(numberOfOrdersReportGenerator, "year") },
                { 9, async () => await GenerateReportData(employeeProductivityReportGenerator) },
                { 10, async () => await GenerateReportData(paymentRatioReportGenerator) }
            };

            reportChartGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReportChart(salesReportChartGenerator) },
                { 1, async () => await GenerateReportChart(revenueReportChartGenerator,r => r.Date.ToString("yyyy-MM-dd")) },
                { 2, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.DayOfWeek.ToString()) },
                { 3, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy-MM")) },
                { 4, async () => await GenerateReportChart(revenueReportChartGenerator, r => r.Date.ToString("yyyy")) },
                { 5, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM-dd")) },
                { 6, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.DayOfWeek.ToString()) },
                { 7, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy-MM")) },
                { 8, async () => await GenerateReportChart(numberOfOrdersReportChartGenerator, o => o.Date.ToString("yyyy")) },
                { 9, async () => await GenerateReportChart(employeeProductivityReportChartGenerator) },
                { 10, async () => await GenerateReportChart(paymentRatioReportChartGenerator) }
            };
        }

        private async Task GeneratePredictionChart<T>(IChartGenerator<T> chartGenerator,
            Func<dynamic, string>? labelSelector = null)
        {
            chartGenerator.GenerateChart(revenuePredictions as List<T>, seriesCollection, out labels, labelSelector);
        }

        private async Task GenerateReportData<T>(IReportGenerator<T> reportGenerator, string? groupBy = null)
        {
            reportData = await reportGenerator.GenerateData(startDate, endDate, groupBy);
        }

        private async Task GenerateReportChart<T>(IChartGenerator<T> chartGenerator,
            Func<dynamic, string>? labelSelector = null)
        {
            chartGenerator.GenerateChart(reportData as List<T>, seriesCollection, out labels, labelSelector);
        }

        public void SetParameters(SeriesCollection seriesCollection, DateTime startDate, DateTime endDate)
        {
            this.seriesCollection = seriesCollection;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public async Task GenerateReport(int selectedReportIndex)
        {
            await reportDataGenerators[selectedReportIndex]();
            await reportChartGenerators[selectedReportIndex]();
        }

        public List<string> GetUpdatedLabelsValues()
        {
            return labels;
        }

        public async Task GeneratePrediction(int selectedReportIndex)
        {
            await reportDataGenerators[selectedReportIndex]();

            var historicalData = ConvertToPredictionData(reportData as List<RevenueReportDto>);

            var predictionModel = new PredictionModel();
            predictionModel.TrainModel(historicalData);

            revenuePredictions = predictionModel.Predict(historicalData);

            await GeneratePredictionChart(predictionChartGenerator, r => r.Date.ToString("yyyy-MM-dd"));
        }

        private List<RevenuePredictionDto> ConvertToPredictionData(List<RevenueReportDto> reportData)
        {
            return reportData.Select(report => new RevenuePredictionDto
            {
                Date = report.Date,
                TotalRevenue = report.TotalRevenue
            }).ToList();
        }
    }
}
