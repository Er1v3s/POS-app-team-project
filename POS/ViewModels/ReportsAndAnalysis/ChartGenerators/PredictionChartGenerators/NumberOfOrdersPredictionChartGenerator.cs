﻿using LiveCharts.Wpf;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;
using System;
using System.Collections.Generic;
using System.Linq;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators
{
    public class NumberOfOrdersPredictionChartGenerator : IChartGenerator<NumberOfOrdersPredictionDto>
    {
        public void GenerateChart(List<NumberOfOrdersPredictionDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            seriesCollection.Add(new ColumnSeries()
            {
                Title = "Prognozowany przychód",
                Values = new ChartValues<float>(data.Select(p => p.NumberOfOrders)),
                PointGeometry = null,
                LabelPoint = point => point.Y.ToString("N"),
                DataLabels = true,
            });

            labels = data.Select(labelSelector).ToList();
        }
    }
}