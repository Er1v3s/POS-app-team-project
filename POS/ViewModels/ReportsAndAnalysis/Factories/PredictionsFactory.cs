using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using Org.BouncyCastle.Asn1.Cms;
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
                { 0, async () => await GeneratePrediction(salePredictionGenerator, 7, 350, 7) }, // not implemented
                { 1, async () => await GeneratePrediction(revenuePredictionGenerator, (absoluteDate - absoluteDate.AddMonths(-2)).Days, (absoluteDate - absoluteDate.AddYears(-1)).Days, 1) },
                { 2, async () => await GeneratePrediction(revenuePredictionGenerator, (absoluteDate - absoluteDate.AddMonths(-2)).Days, (absoluteDate - absoluteDate.AddYears(-1)).Days, 7) },
                { 3, async () => await GeneratePrediction(revenuePredictionGenerator, (absoluteDate - absoluteDate.AddYears(-1)).Days, (absoluteDate - absoluteDate.AddYears(-3)).Days, (absoluteDate.AddMonths(1) - absoluteDate).Days) }, // to change
                { 4, async () => await GeneratePrediction(revenuePredictionGenerator, 7, 28, 365) }, // to change
            };

            _predictionParameters = new Dictionary<int, Action>
            {
                // The parameters of the SetParameters method should not be static, but they are, because the report generator could accept empty data because the database is no longer updated.
                { 0, () => _reportsFactory.SetParameters(DateTime.Now, DateTime.Now) }, // not implemented 
                { 1, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-1).AddMonths(-1), absoluteDate.AddMonths(-1)) },
                { 2, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-1).AddMonths(-1), absoluteDate.AddMonths(-1)) },
                { 3, () => _reportsFactory.SetParameters(absoluteDate.AddYears(-3).AddYears(-1), absoluteDate.AddMonths(-1)) }, // to change
                { 4, () => _reportsFactory.SetParameters(absoluteDate, absoluteDate) }, // to change
            };
        }

        private async Task GeneratePrediction<TInput, TOutput>(IPredictionGenerator<TInput, TOutput> predictionGenerator, int windowSize, int seriesLength, int horizon)
        {
            var data = _reportsFactory.GetReportData() as List<TInput>;

            _revenuePredictions = predictionGenerator.GeneratePrediction(data, windowSize, seriesLength, horizon);
        }

        public async Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection)
        {
            _predictionParameters[selectedReportIndex]();
            await _reportsFactory.GenerateReport(1); // THIS PARAMETR MUST BE 1, BECAUSE ONLY THIS RETURNS DATA WITH EACH DAYS.
            await _predictionGenerators[selectedReportIndex]();
        }

        public object GetPredictionData()
        {
            return _revenuePredictions;
        }
    }
}
