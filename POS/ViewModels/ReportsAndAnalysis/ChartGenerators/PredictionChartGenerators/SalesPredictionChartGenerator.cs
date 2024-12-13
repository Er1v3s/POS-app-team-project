using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using System;
using System.Collections.Generic;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;
using LiveCharts.Wpf;
using System.Linq;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators
{
    public class SalesPredictionChartGenerator : IChartGenerator<ProductSalesPredictionDto>
    {
        public void GenerateChart(List<ProductSalesPredictionDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            var dataGrouped = GroupDataByProductNames(data);

            seriesCollection.Add(new ColumnSeries
            {
                Title = "Prognoza sprzedaży",
                Values = new ChartValues<float>(dataGrouped.Select(p => p.PredictedQuantity)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true
            });

            labels = dataGrouped.Select(p => p.ProductName).ToList();
        }

        private List<ProductSalesPredictionDto> GroupDataByProductNames(List<ProductSalesPredictionDto> orderedItems)
        {
            return orderedItems
                .GroupBy(item => item.ProductName)
                .Select(group => new ProductSalesPredictionDto()
                {
                    ProductName = group.First().ProductName,
                    PredictedQuantity = group.Sum(item => item.PredictedQuantity)
                })
                .ToList();
        }
    }

}
