using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators
{
    public class NumberOfOrdersChartGenerator : IChartGenerator<OrderReportDto>
    {
        public void GenerateChart(List<OrderReportDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = "Liczba zamówień",
                Values = new ChartValues<int>(data.Select(o => o.OrderCount)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            labels = data.Select(labelSelector).ToList();
        }
    }
}
