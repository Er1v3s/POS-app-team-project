using System;
using System.Collections.Generic;
using LiveCharts;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IChartGenerator<T>
    {
        void GenerateChart(List<T> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null);
    }
}
