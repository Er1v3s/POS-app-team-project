using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators
{
    public class RevenueChartGenerator : IChartGenerator<RevenueReportDto>
    {
        public void GenerateChart(List<RevenueReportDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = "Przychód",
                Values = new ChartValues<float>(data.Select(p => p.TotalRevenue)),
                LabelPoint = point => point.Y.ToString("C"),
                DataLabels = true,
            });

            labels = data.Select(labelSelector).ToList();
        }
    }
}
