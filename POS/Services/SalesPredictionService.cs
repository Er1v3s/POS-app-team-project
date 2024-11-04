using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;

namespace POS.Services
{
    public class SalesPredictionService
    {
        private readonly MLContext _mlContext;

        public SalesPredictionService()
        {
            _mlContext = new MLContext();
        }

        public ITransformer TrainModel(List<RevenueReportDto> revenueData)
        {
            // Konwertowanie listy na IDataView
            IDataView dataView = _mlContext.Data.LoadFromEnumerable(revenueData);

            // Pipeline modelu do prognozowania
            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(RevenuePrediction.ForecastedRevenue),
                inputColumnName: nameof(RevenueReportDto.TotalRevenue),
                windowSize: 12,
                seriesLength: 24,
                trainSize: revenueData.Count,
                horizon: 12,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(RevenuePrediction.LowerBoundRevenue),
                confidenceUpperBoundColumn: nameof(RevenuePrediction.UpperBoundRevenue)
            );

            // Trenowanie modelu
            var model = forecastingPipeline.Fit(dataView);
            return model;
        }

        public RevenuePrediction Predict(ITransformer model, List<RevenueReportDto> revenueData)
        {
            // Tworzenie silnika prognozującego na podstawie modelu
            var forecastingEngine = model.CreateTimeSeriesEngine<RevenueReportDto, RevenuePrediction>(_mlContext);

            // Predykcja
            return forecastingEngine.Predict();
        }
    }
}
