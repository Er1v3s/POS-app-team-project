using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class RevenuePredictionGenerator : IPredictionGenerator<RevenueReportDto, RevenuePredictionDto>
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public RevenuePredictionGenerator()
        {
            _mlContext = new MLContext();
        }

        private void TrainModel(List<RevenuePredictionDto> revenueData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(revenueData);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(RevenuePredictionDataModel.PredictedRevenue),
                inputColumnName: nameof(RevenuePredictionDto.TotalRevenue),
                windowSize: 7,     // Define based on your time-series pattern
                seriesLength: 28,  // Series length should match the data pattern
                trainSize: 28,    // Number of records to train on
                horizon: 7         // Predicting one week ahead
            );

            _model = pipeline.Fit(dataView);
        }

        private List<RevenuePredictionDto> Predict()
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<RevenuePredictionInput, RevenuePredictionDataModel>(_mlContext);
            var forecast = forecastEngine.Predict();

            List<RevenuePredictionDto> predictions = new List<RevenuePredictionDto>();

            for (int i = 0; i < forecast.PredictedRevenue.Length; i++)
            {
                predictions.Add(new RevenuePredictionDto
                {
                    Date = DateTime.Now.AddDays(i + 1),
                    TotalRevenue = forecast.PredictedRevenue[i]
                });
            }

            return predictions;
        }

        public List<RevenuePredictionDto> GeneratePrediction(List<RevenueReportDto> data)
        {
            var historicalData = ConvertToPredictionData(data);

            TrainModel(historicalData);

            var revenuePredictions = Predict();

            return revenuePredictions;
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
