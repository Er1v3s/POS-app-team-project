using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IChartGenerator<in T>
    {
        void GenerateChart(IQueryable<T> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null);
    }
}
