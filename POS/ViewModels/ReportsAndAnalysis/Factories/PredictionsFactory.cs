using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.Factories
{
    public class PredictionsFactory : IPredictionsFactory
    {
        private readonly Dictionary<int, Func<Task>> _predictionGenerators;
        private readonly Dictionary<int, Action> _predictionParameters;

        private readonly IReportsFactory _reportsFactory;

        private object _revenuePredictions;

        private DateTime absoluteDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public PredictionsFactory(
            IReportsFactory reportFactory,

            IPredictionGenerator<RevenueReportDto, RevenuePredictionDto> revenuePredictionGenerator,
            IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto> salePredictionGenerator)
        {
            _reportsFactory = reportFactory;

            _predictionGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GeneratePrediction(salePredictionGenerator, 7, 350, 7, GroupBy.Day) }, // not implemented
                { 1, async () => await GeneratePrediction(revenuePredictionGenerator, (absoluteDate - absoluteDate.AddMonths(-2)).Days, (absoluteDate - absoluteDate.AddYears(-1)).Days, 1, GroupBy.Day) },
                { 2, async () => await GeneratePrediction(revenuePredictionGenerator, (absoluteDate - absoluteDate.AddMonths(-2)).Days, (absoluteDate - absoluteDate.AddYears(-1)).Days, 7, GroupBy.Day) },
                { 3, async () => await GeneratePrediction(revenuePredictionGenerator, 12, 36, 6, GroupBy.Month) },
                { 4, async () => await GeneratePrediction(revenuePredictionGenerator, 2, 6, 1, GroupBy.Year) },
            };

            _predictionParameters = new Dictionary<int, Action>
            {
                // The parameters of the SetParameters method should not be static, but they are, because the report generator could accept empty data because the database is no longer updated.
                { 0, () => _reportsFactory.SetParameters(DateTime.Now, DateTime.Now) }, // not implemented 
                { 1, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-1).AddMonths(-1), absoluteDate.AddMonths(-1)) },
                { 2, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-1).AddMonths(-1), absoluteDate.AddMonths(-1)) },
                { 3, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-3).AddMonths(-(absoluteDate.Month - 1)).AddDays(-(absoluteDate.Day - 1)), absoluteDate.AddDays(-(absoluteDate.Day - 1))) },
                { 4, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-6).AddMonths(-(absoluteDate.Month - 1)).AddDays(-(absoluteDate.Day - 1)), absoluteDate.AddMonths(-(absoluteDate.Month - 1)).AddDays(-(absoluteDate.Day - 1))) },
            };
        }

        public async Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection)
        {
            // Prediction for weeks requires daily report, not weekly 
            var reportIndex = selectedReportIndex == 2 ? 1 : selectedReportIndex;

            _predictionParameters[selectedReportIndex]();
            await _reportsFactory.GenerateReport(reportIndex);
            await _predictionGenerators[selectedReportIndex]();
        }

        public object GetPredictionData()
        {
            return _revenuePredictions;
        }

        private async Task GeneratePrediction<TInput, TOutput>(IPredictionGenerator<TInput, TOutput> predictionGenerator, int windowSize, int seriesLength, int horizon, GroupBy groupBy)
        {
            var data = _reportsFactory.GetReportData() as List<TInput>;

            _revenuePredictions = predictionGenerator.GeneratePrediction(data, windowSize, seriesLength, horizon, groupBy);
        }
    }
}
