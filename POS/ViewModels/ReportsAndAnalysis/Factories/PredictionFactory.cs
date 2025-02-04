using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.Factories
{
    public class PredictionFactory : IPredictionsFactory
    {
        private readonly Dictionary<int, Func<Task>> _predictionGenerators;
        private readonly Dictionary<int, Action> _predictionParameters;

        private readonly IReportsFactory _reportsFactory;

        private object prediction;

        private readonly DateTime predictionRange = new (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private readonly DateTime reportDateRange = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public PredictionFactory(
            IReportsFactory reportFactory,

            IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto> salePredictionGenerator,
            IPredictionGenerator<RevenueReportDto, RevenuePredictionDto> revenuePredictionGenerator,
            IPredictionGenerator<OrderReportDto, NumberOfOrdersPredictionDto> numberOfOrdersPredictionGenerator)
        {
            _reportsFactory = reportFactory;

            _predictionGenerators = new Dictionary<int, Func<Task>>
            {
                // These parameters of GeneratePrediction method are correct and I shouldn't touch them
                { 0, async () => await GeneratePredictionAsync(salePredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 1, GroupBy.Day) },
                { 1, async () => await GeneratePredictionAsync(salePredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 7, GroupBy.Day) }, // testing seriesLength
                { 2, async () => await GeneratePredictionAsync(salePredictionGenerator, 12, 1, GroupBy.Month) }, 
                { 3, async () => await GeneratePredictionAsync(salePredictionGenerator, 2,1, GroupBy.Year) }, 
                { 4, async () => await GeneratePredictionAsync(revenuePredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 1, GroupBy.Day) },
                { 5, async () => await GeneratePredictionAsync(revenuePredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 7, GroupBy.Day) },
                { 6, async () => await GeneratePredictionAsync(revenuePredictionGenerator, 12, 6, GroupBy.Month) },
                { 7, async () => await GeneratePredictionAsync(revenuePredictionGenerator, 2, 1, GroupBy.Year) },
                { 8, async () => await GeneratePredictionAsync(numberOfOrdersPredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 1, GroupBy.Day) },
                { 9, async () => await GeneratePredictionAsync(numberOfOrdersPredictionGenerator, (predictionRange - predictionRange.AddMonths(-2)).Days, 7, GroupBy.Day) },
                { 10, async () => await GeneratePredictionAsync(numberOfOrdersPredictionGenerator, 12, 6, GroupBy.Month) },
                { 11, async () => await GeneratePredictionAsync(numberOfOrdersPredictionGenerator, 2, 1, GroupBy.Year) },
            };

            // AddMonths(-2) is temporary because the database is no longer updated!!! when you seed database correctly you should delete this.
            reportDateRange = reportDateRange.AddMonths(-2).AddDays(1);
            //reportDateRange = reportDateRange.AddDays(3);
            // AddMonths(-2) is temporary because the database is no longer updated!!! when you seed database correctly you should delete this.

            _predictionParameters = new Dictionary<int, Action>
            {
                // .AddDays(-(reportDateRange.Day - 1))
                // .AddMonths(-(reportDateRange.Month - 1))
                // Methods are required because when we generate report we want report from whole month or year. With this we get date like: 01.01.2023 not like 07.12.2023 as a start date.

                { 0, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 1, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 2, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-3).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddDays(-(reportDateRange.Day - 1))) },
                { 3, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-6).AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1))) },
                { 4, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 5, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 6, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-3).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddDays(-(reportDateRange.Day - 1))) },
                { 7, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-6).AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1))) },
                { 8, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 9, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-1), reportDateRange) },
                { 10, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-3).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddDays(-(reportDateRange.Day - 1))) },
                { 11, () => _reportsFactory.SetParameters(reportDateRange.AddYears(-6).AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1)), reportDateRange.AddMonths(-(reportDateRange.Month - 1)).AddDays(-(reportDateRange.Day - 1))) },
            };
        }

        public async Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection)
        {
            // Prediction for weeks requires daily report, not weekly 
            var reportIndex = selectedReportIndex;

            if (selectedReportIndex == 5)
                reportIndex = 4;
            if (selectedReportIndex == 9)
                reportIndex = 8;

            _predictionParameters[selectedReportIndex]();
            await _reportsFactory.GenerateReport(reportIndex);
            await _predictionGenerators[selectedReportIndex]();
        }

        public object GetPredictionData()
        {
            return prediction;
        }

        private async Task GeneratePredictionAsync<TInput, TOutput>(IPredictionGenerator<TInput, TOutput> predictionGenerator, int windowSize, int horizon, GroupBy groupBy)
        {
            var data = _reportsFactory.GetReportData() as List<TInput>;

            prediction = await predictionGenerator.GeneratePrediction(data, windowSize, horizon, groupBy);
        }
    }
}
