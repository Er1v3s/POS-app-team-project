using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators
{
    public class EmployeeProductivityChartGenerator : IChartGenerator<EmployeeProductivityDto>
    {
        public void GenerateChart(List<EmployeeProductivityDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = "Ilość zrealizowanych zamówień: ",
                Values = new ChartValues<int>(data.Select(p => p.OrderCount)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            seriesCollection.Add(new ColumnSeries
            {
                Title = "Suma kwot zamówień: ",
                Values = new ChartValues<double>(data.Select(p => p.TotalAmount)),
                LabelPoint = point => point.Y.ToString("C"),
                DataLabels = true,
            });

            labels = data.Select(p => p.EmployeeName).ToList();
        }
    }
}
