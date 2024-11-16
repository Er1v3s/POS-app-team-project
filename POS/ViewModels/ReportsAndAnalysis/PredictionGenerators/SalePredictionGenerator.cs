using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ML;
    using Microsoft.ML.Transforms.TimeSeries;

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

            private void TrainModel(List<ProductSalesPredictionInput> salesData)
            {
                var dataView = _mlContext.Data.LoadFromEnumerable(salesData);

                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(ProductSalesPredictionDataModel.PredictedQuantity),
                    inputColumnName: nameof(ProductSalesPredictionInput.Quantity),
                    windowSize: 7,     // Define based on your time-series pattern
                    seriesLength: 28,  // Match historical data range
                    trainSize: 28,     // Train on the last 28 records
                    horizon: 7         // Predict for the next 7 days
                );

                _model = pipeline.Fit(dataView);
            }

            private List<ProductSalesPredictionDto> Predict(string productName)
            {
                var forecastEngine = _model.CreateTimeSeriesEngine<ProductSalesPredictionInput, ProductSalesPredictionDataModel>(_mlContext);
                var forecast = forecastEngine.Predict();

                List<ProductSalesPredictionDto> predictions = new List<ProductSalesPredictionDto>();

                for (int i = 0; i < forecast.PredictedQuantity.Length; i++)
                {
                    predictions.Add(new ProductSalesPredictionDto
                    {
                        ProductName = productName,
                        Quantity = (int)Math.Round(forecast.PredictedQuantity[i])
                    });
                }

                return predictions;
            }

            public List<ProductSalesPredictionDto> GeneratePrediction(List<ProductSalesDto> data)
            {
                var groupedData = data.GroupBy(d => d.ProductName)
                    .ToDictionary(g => g.Key, g => g.ToList());

                List<ProductSalesPredictionDto> allPredictions = new List<ProductSalesPredictionDto>();

                foreach (var product in groupedData)
                {
                    var historicalData = ConvertToPredictionData(product.Value);

                    TrainModel(historicalData);

                    var productPredictions = Predict(product.Key);

                    allPredictions.AddRange(productPredictions);
                }

                return allPredictions;
            }

            private List<ProductSalesPredictionInput> ConvertToPredictionData(List<ProductSalesDto> salesData)
            {
                return salesData.Select(sale => new ProductSalesPredictionInput
                {
                    ProductName = sale.ProductName,
                    Quantity = sale.Quantity
                }).ToList();
            }
        }
    }

}
