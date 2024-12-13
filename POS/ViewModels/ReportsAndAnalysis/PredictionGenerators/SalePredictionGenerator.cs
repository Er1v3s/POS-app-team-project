using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class ProductSalesPredictionGenerator : PredictionGenerator<ProductSalesDto>, IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>
    {
        public async Task<List<ProductSalesPredictionDto>> GeneratePrediction(List<ProductSalesDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var dataGroupedByName = data.GroupBy(d => d.ProductName);

            var predictions = new List<ProductSalesPredictionDto>();

            foreach (var groupedData in dataGroupedByName)
            {
                var timeSeriesData = PrepareTimeSeriesData(groupedData);

                await Task.Run(() =>TrainModel(timeSeriesData, windowSize, horizon));

                var prediction = await Predict(groupedData.Key);
                predictions.AddRange(prediction);
            }

            await Task.WhenAll();

            return predictions;
        }

        private async Task<List<ProductSalesPredictionDto>> Predict(string productName)
        {
            var forecast = await Task.Run(GenerateForecast);

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
                }).ToList();
        }
    }
}