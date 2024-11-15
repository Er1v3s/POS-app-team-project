using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports.ReportsPredictions;
using System;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.Models.Reports;
using System.Linq;

namespace POS.ViewModels.ReportsAndAnalysis.Predictions
{
    public class PredictionGenerator : IPredictionGenerator<RevenueReportDto>
    {
        private MLContext mlContext;
        private ITransformer model;

        public PredictionGenerator()
        {
            mlContext = new MLContext();
        }

        private void TrainModel(List<RevenuePredictionDto> revenueData)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(revenueData);

            var pipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(RevenuePredictionDataModel.PredictedRevenue),
                inputColumnName: nameof(RevenuePredictionInput.TotalRevenue),
                windowSize: 7,     // Define based on your time-series pattern
                seriesLength: 30,  // Series length should match the data pattern
                trainSize: 365,    // Number of records to train on
                horizon: 7         // Predicting one week ahead
            );

            model = pipeline.Fit(dataView);
        }

        private List<RevenuePredictionDto> Predict()
        {
            var forecastEngine = model.CreateTimeSeriesEngine<RevenuePredictionInput, RevenuePredictionDataModel>(mlContext);
            var forecast = forecastEngine.Predict();

            List<RevenuePredictionDto> predictions = new List<RevenuePredictionDto>();

            for (int i = 0; i < forecast.PredictedRevenue.Length; i++)
            {
                predictions.Add(new RevenuePredictionDto
                {
                    Date = DateTime.Now.AddDays(i + 1), // Future date for each forecast point
                    TotalRevenue = forecast.PredictedRevenue[i] // Forecasted revenue
                });
            }

            return predictions;
        }

        public List<RevenuePredictionDto> GeneratePrediction(List<RevenueReportDto> data)
        {
            var historicalData = ConvertToPredictionData(data);

            var predictionModel = new PredictionGenerator();
            predictionModel.TrainModel(historicalData);

            var revenuePredictions = predictionModel.Predict();

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
