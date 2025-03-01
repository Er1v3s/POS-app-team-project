﻿using System.Collections.Generic;
using LiveCharts;
using POS.Models.Reports;

namespace POS.Services.ReportsAndAnalysis.Interfaces
{
    public interface IChartsFactory
    {
        List<string> GetUpdatedLabelsValues();
        void GenerateChart(int selectedReportIndex, SeriesCollection seriesCollection, ChartType chartType);
    }
}
