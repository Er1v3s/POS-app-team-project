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
        public void GenerateChart(
            List<ProductSalesPredictionDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = "Prognoza sprzedaży",
                Values = new ChartValues<float>(data.Select(p => p.PredictedQuantity)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true
            });

            labels = data.Select(p => p.ProductName).ToList();
        }
    }

}
