using System.Collections.Generic;
using Microsoft.ML;
using System.Linq;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using Microsoft.ML.Data;
using System;

namespace POS.ViewModels.ReportsAndAnalysis.Predictions
{
    public class PredictionModel
    {
        private MLContext mlContext;
        private ITransformer model;

        public PredictionModel()
        {
            mlContext = new MLContext();
        }

        public void TrainModel(List<RevenuePredictionDto> revenueData)
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

        public List<RevenuePredictionDto> Predict(List<RevenuePredictionDto> historicalData)
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
    }
}
