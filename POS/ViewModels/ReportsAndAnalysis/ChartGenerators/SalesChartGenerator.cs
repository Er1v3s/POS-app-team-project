using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators
{
    public class SalesChartGenerator : IChartGenerator<ProductSalesDto>
    {
        public void GenerateChart(List<ProductSalesDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = "Ilość sprzedanych produktów: ",
                Values = new ChartValues<int>(data.Select(p => (p.Quantity))),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            labels = data.Select(p => p.ProductName).ToList();
        }
    }
}
