using System;
using System.Collections.Generic;
using System.Linq;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class ProductSalesPredictionGenerator : PredictionGenerator<ProductSalesDto>, IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>
    {
        public IQueryable<ProductSalesPredictionDto> GeneratePrediction(IQueryable<ProductSalesDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var dataGroupedByName = data.GroupBy(d => d.ProductName);

            var predictions = new List<ProductSalesPredictionDto>();

            foreach (var groupedData in dataGroupedByName)
            {
                var timeSeriesData = PrepareTimeSeriesData(groupedData.AsQueryable());

                TrainModel(timeSeriesData, windowSize, horizon);

                var prediction = Predict(groupedData.Key);
                predictions.AddRange(prediction);
            }

            return predictions.AsQueryable();
        }

        private IQueryable<ProductSalesPredictionDto> Predict(string productName)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, productName);

            return formattedPrediction;
        }

        private IQueryable<ProductSalesPredictionDto> SetDataFormat(PredictionDataModel forecast, string productName)
        {
            return Enumerable.Range(0, forecast.Total.Length)
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
                }).AsQueryable();
        }
    }
}