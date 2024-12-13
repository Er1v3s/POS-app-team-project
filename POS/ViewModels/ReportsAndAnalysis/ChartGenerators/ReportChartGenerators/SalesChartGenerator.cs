using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators
{
    public class SalesChartGenerator : IChartGenerator<ProductSalesDto>
    {
        public void GenerateChart(List<ProductSalesDto> data, SeriesCollection seriesCollection, out List<string> labels, Func<dynamic, string>? labelSelector = null)
        {
            var dataGrouped = GroupDataByProductNames(data);

            seriesCollection.Add(new ColumnSeries
            {
                Title = "Ilość sprzedanych produktów: ",
                Values = new ChartValues<int>(dataGrouped.Select(p => p.Quantity)),
                LabelPoint = point => point.Y.ToString("N0"),
                DataLabels = true,
            });

            labels = dataGrouped.Select(p => p.ProductName).ToList();
        }

        private List<ProductSalesDto> GroupDataByProductNames(List<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .GroupBy(item => item.ProductName)
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.First().ProductName,
                    Quantity = group.Sum(item => item.Quantity)
                })
                .ToList();
        }
    }
}
