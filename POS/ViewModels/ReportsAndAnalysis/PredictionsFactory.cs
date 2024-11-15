using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class PredictionsFactory : IPredictionsFactory
    {
        private readonly Dictionary<int, Func<Task>> _predictionGenerators;

        private readonly IReportsFactory _reportsFactory;

        private List<RevenuePredictionDto> _revenuePredictions;

        public PredictionsFactory(
            IReportsFactory reportFactory,

            IPredictionGenerator<RevenueReportDto> predictionGenerator)
        {
            _reportsFactory = reportFactory;

            _predictionGenerators = new Dictionary<int, Func<Task>>
            {
                { 1, async () => await GeneratePrediction(predictionGenerator) },
            };
        }

        private async Task GeneratePrediction<T>(IPredictionGenerator<T> predictionGenerator)
        {
            var data = _reportsFactory.GetReportData() as List<T>;

            _revenuePredictions = predictionGenerator.GeneratePrediction(data);
        }

        public async Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection)
        {
            await _predictionGenerators[selectedReportIndex]();
        }

        public List<RevenuePredictionDto> GetPredictionData()
        {
            return _revenuePredictions;
        }
    }
}
