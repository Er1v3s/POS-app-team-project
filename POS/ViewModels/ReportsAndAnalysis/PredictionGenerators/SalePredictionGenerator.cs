using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class ProductSalesPredictionGenerator : IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public ProductSalesPredictionGenerator()
        {
            _mlContext = new MLContext();
        }

        public List<ProductSalesPredictionDto> GeneratePrediction(List<ProductSalesDto> data, int windowSize,
            int seriesLength, int horizon, GroupBy groupBy)
        {
            var dataGroupedByName = data.GroupBy(d => d.ProductName).ToList();

            var predictions = new List<ProductSalesPredictionDto>();

            foreach (var groupedData in dataGroupedByName)
            {
                var timeSeriesData = PrepareTimeSeriesData(groupedData.ToList());

                TrainModel(timeSeriesData, windowSize, horizon);

                var productPredictions = Predict(groupedData.Key, groupBy);
                predictions.AddRange(productPredictions);
            }

            return predictions;
        }

        private List<SalesData> PrepareTimeSeriesData(List<ProductSalesDto> data)
        {
            return data
                
                .Select(g => new SalesData
                {
                    Date = g.Date,
                    Quantity = g.Quantity
                })
                .ToList();
        }

        private void TrainModel(List<SalesData> data, int windowSize, int horizon)
        {
            var trainData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(PredictionDataModel.Total),
                inputColumnName: nameof(SalesData.Quantity),
                windowSize: windowSize,
                seriesLength: data.Count,
                trainSize: (int)Math.Round(data.Count * 0.8),
                horizon: horizon);

            _model = pipeline.Fit(trainData);
        }

        private List<ProductSalesPredictionDto> Predict(string productName, GroupBy groupBy)
        {
            var forecast = GenerateForecast();

            var predictedQuantities = Enumerable.Range(0, forecast.Total.Length)
                .Select(i => new ProductSalesPredictionDto
                {
                    ProductName = productName,
                    PredictedQuantity = forecast.Total[i]
                })
                .GroupBy(p => p.ProductName)
                .Select(group => new ProductSalesPredictionDto
                {
                    ProductName = group.Key,
                    PredictedQuantity = (int)Math.Round(group.Sum(p => p.PredictedQuantity))
                })
                .ToList();

            return predictedQuantities;
        }

        private PredictionDataModel GenerateForecast()
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<SalesData, PredictionDataModel>(_mlContext);
            var forecast = forecastEngine.Predict();

            return forecast;
        }
    }
}