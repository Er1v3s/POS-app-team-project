using System;
using System.Collections.Generic;
using Microsoft.ML;
using System.Linq;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class NumberOfOrdersPredictionGenerator : IPredictionGenerator<OrderReportDto, NumberOfOrdersPredictionDto>
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public NumberOfOrdersPredictionGenerator()
        {
            _mlContext = new MLContext();
        }

        public List<NumberOfOrdersPredictionDto> GeneratePrediction(List<OrderReportDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var historicalData = ConvertToPredictionData(data);

            TrainModel(historicalData, windowSize, horizon);

            var prediction = Predict(groupBy);

            return prediction;
        }

        private void TrainModel(List<NumberOfOrdersPredictionDto> data, int windowSize, int horizon)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(PredictionDataModel.Total),
                inputColumnName: nameof(NumberOfOrdersPredictionDto.NumberOfOrders),
                windowSize: windowSize,     // Define based on your time-series pattern
                seriesLength: data.Count,  // Series length should match the data pattern
                trainSize: (int)Math.Round(data.Count * 0.8),    // Number of records to train on
                horizon: horizon         // Predicting one week ahead
            );

            _model = pipeline.Fit(dataView);
        }

        private List<NumberOfOrdersPredictionDto> Predict(GroupBy groupBy)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, groupBy);

            return formattedPrediction;
        }

        private List<NumberOfOrdersPredictionDto> ConvertToPredictionData(List<OrderReportDto> reportData)
        {
            return reportData.Select(report => new NumberOfOrdersPredictionDto
            {
                Date = report.Date,
                NumberOfOrders = report.OrderCount
            }).ToList();
        }

        private PredictionDataModel GenerateForecast()
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<NumberOfOrdersPredictionDto, PredictionDataModel>(_mlContext);
            var forecast = forecastEngine.Predict();

            return forecast;
        }

        private List<NumberOfOrdersPredictionDto> SetDataFormat(PredictionDataModel forecast, GroupBy groupBy)
        {
            var predictions = new List<NumberOfOrdersPredictionDto>();

            for (int i = 0; i < forecast.Total.Length; i++)
            {
                switch (groupBy)
                {
                    case GroupBy.Day:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddDays(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                    case GroupBy.Month:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddMonths(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                    case GroupBy.Year:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddYears(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                }
            }

            return predictions;
        }
    }
}
