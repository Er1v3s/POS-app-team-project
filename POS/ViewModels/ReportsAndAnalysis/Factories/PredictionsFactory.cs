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

        private readonly IReportsFactory _reportsFactory;

        private object _revenuePredictions;

        public PredictionsFactory(
            IReportsFactory reportFactory,

            IPredictionGenerator<RevenueReportDto, RevenuePredictionDto> revenuePredictionGenerator,
            IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto> salePredictionGenerator)
        {
            _reportsFactory = reportFactory;

            _predictionGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GeneratePrediction(salePredictionGenerator) },
                { 1, async () => await GeneratePrediction(revenuePredictionGenerator) },
            };
        }

        private async Task GeneratePrediction<TInput, TOutput>(IPredictionGenerator<TInput, TOutput> predictionGenerator)
        {
            var data = _reportsFactory.GetReportData() as List<TInput>;

            _revenuePredictions = predictionGenerator.GeneratePrediction(data);
        }

        public async Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection)
        {
            await _predictionGenerators[selectedReportIndex]();
        }

        public object GetPredictionData()
        {
            return _revenuePredictions;
        }
    }
}
