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

        public List<RevenuePredictionDto> GeneratePrediction(List<RevenueReportDto> data, int windowSize, int seriesLength, int horizon, GroupBy groupBy)
        {
            var historicalData = ConvertToPredictionData(data);

            TrainModel(historicalData, windowSize, seriesLength, horizon);

            var revenuePredictions = Predict(groupBy);

            return revenuePredictions;
        }

        private void TrainModel(List<RevenuePredictionDto> revenueData, int windowSize, int seriesLength, int horizon)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(revenueData);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(RevenuePredictionDataModel.PredictedRevenue),
                inputColumnName: nameof(RevenuePredictionDto.TotalRevenue),
                windowSize: windowSize,     // Define based on your time-series pattern
                seriesLength: seriesLength,  // Series length should match the data pattern
                trainSize: (int)Math.Round(seriesLength * 0.8),    // Number of records to train on
                horizon: horizon         // Predicting one week ahead
            );

            _model = pipeline.Fit(dataView);
        }

        private List<RevenuePredictionDto> Predict(GroupBy groupBy)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, groupBy);

            return formattedPrediction;
        }

        private List<RevenuePredictionDto> ConvertToPredictionData(List<RevenueReportDto> reportData)
        {
            return reportData.Select(report => new RevenuePredictionDto
            {
                Date = report.Date,
                TotalRevenue = report.TotalRevenue
            }).ToList();
        }

        private RevenuePredictionDataModel GenerateForecast()
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<RevenuePredictionInput, RevenuePredictionDataModel>(_mlContext);
            var forecast = forecastEngine.Predict();

            return forecast;
        }

        private List<RevenuePredictionDto> SetDataFormat(RevenuePredictionDataModel forecast, GroupBy groupBy)
        {
            List<RevenuePredictionDto> predictions = new List<RevenuePredictionDto>();

            for (int i = 0; i < forecast.PredictedRevenue.Length; i++)
            {
                switch (groupBy)
                {
                    case GroupBy.Day:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddDays(i + 1),
                            TotalRevenue = forecast.PredictedRevenue[i]
                        });
                        break;
                    case GroupBy.Month:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddMonths(i + 1),
                            TotalRevenue = forecast.PredictedRevenue[i]
                        });
                        break;
                    case GroupBy.Year:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddYears(i + 1),
                            TotalRevenue = forecast.PredictedRevenue[i]
                        });
                        break;
                }
            }

            return predictions;
        }
    }
}
