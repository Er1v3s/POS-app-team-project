using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators
{
    public class PredictionChart : IChartGenerator<RevenuePredictionDto>
    {
        public void GenerateChart(List<RevenuePredictionDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new LineSeries()
            {
                Title = "Prognozowany przychód",
                Values = new ChartValues<float>(data.Select(p => p.TotalRevenue)),
                PointGeometry = null,
                LabelPoint = point => point.Y.ToString("C"),
                DataLabels = true,
            });

            labels = data.Select(labelSelector).ToList();
        }
    }
}
