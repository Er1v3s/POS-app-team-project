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
                    outputColumnName: nameof(ProductSalesPredictionOutput.PredictedQuantity),
                    inputColumnName: nameof(ProductSalesPredictionInput.Quantity),
                    windowSize: 7,      // Okno czasowe (np. 7 dni)
                    seriesLength: 28,   // Długość serii danych (28 dni)
                    trainSize: 28,      // Dane do trenowania (28 dni)
                    horizon: 1          // Prognozuj tylko na 1 dzień
                );

                _model = pipeline.Fit(dataView);
            }


            private ProductSalesPredictionDto Predict(string productName)
            {
                var forecastEngine = _model.CreateTimeSeriesEngine<ProductSalesPredictionInput, ProductSalesPredictionOutput>(_mlContext);
                var forecast = forecastEngine.Predict();

                return new ProductSalesPredictionDto
                {
                    ProductName = productName,
                    PredictedDate = DateTime.Now.AddDays(1), // Prognoza na jutro
                    PredictedQuantity = forecast.PredictedQuantity[0] // Pierwsza wartość w prognozie
                };
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

                    var productPrediction = Predict(product.Key);

                    allPredictions.Add(productPrediction);
                }

                return allPredictions;
            }


            private List<ProductSalesPredictionInput> ConvertToPredictionData(List<ProductSalesDto> salesData)
            {
                return salesData.Select(sale => new ProductSalesPredictionInput
                {
                    Quantity = sale.Quantity
                }).ToList();
            }

        }
    }

}
