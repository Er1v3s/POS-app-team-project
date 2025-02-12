using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using POS.Models.Reports;
using POS.Services.ReportsAndAnalysis.Interfaces;

namespace POS.Services.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators
{
    public class PaymentMethodRatioChartGenerator : IChartGenerator<PaymentRatioDto>
    {
        public void GenerateChart(List<PaymentRatioDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries()
            {
                Title = "Suma kwot zamówień: ",
                Values = new ChartValues<int>(data.Select(p => p.Count)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            labels = data.Select(p => p.PaymentMethod).ToList();
        }
    }
}
