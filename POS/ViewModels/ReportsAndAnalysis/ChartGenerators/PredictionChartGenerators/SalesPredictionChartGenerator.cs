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
            seriesCollection.Add(new LineSeries()
            {
                Title = "Ilość sprzedanych produktów: ",
                Values = new ChartValues<int>(data.Select(p => p.Quantity)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            labels = data.Select(p => p.ProductName).ToList();
        }
    }
}
