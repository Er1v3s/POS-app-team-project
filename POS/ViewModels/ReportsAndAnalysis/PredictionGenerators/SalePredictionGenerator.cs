using System;
using System.Collections.Generic;
using System.Linq;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    // TO DO 
    // 1. Poprawić wydajność działania Generatorów predykcji stosując IEnumerable
    // 3. Poprawić czytelność kodu 
    // 4. Ogarnąć temat z niezaseedowaną bazą danych!

    public class ProductSalesPredictionGenerator : PredictionGenerator<ProductSalesDto>, IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>
    {
        public List<ProductSalesPredictionDto> GeneratePrediction(List<ProductSalesDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var dataGroupedByName = data.GroupBy(d => d.ProductName).ToList();

            var predictions = new List<ProductSalesPredictionDto>();

            foreach (var groupedData in dataGroupedByName)
            {
                var timeSeriesData = PrepareTimeSeriesData(groupedData.ToList());

                TrainModel(timeSeriesData, windowSize, horizon);

                var prediction = Predict(groupedData.Key);
                predictions.AddRange(prediction);
            }

            return predictions;
        }

        private List<ProductSalesPredictionDto> Predict(string productName)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, productName);

            return formattedPrediction;
        }

        private List<ProductSalesPredictionDto> SetDataFormat(PredictionDataModel forecast, string productName)
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
                })
                .ToList();
        }
    }
}