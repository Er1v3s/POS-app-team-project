using System.Collections.Generic;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ChartGenerators
{
    public class SalesChartGenerator : IChartGenerator<ProductSalesDto>
    {
        public void GenerateChar(List<ProductSalesDto> data)
        {
            //SeriesCollection.Add(new ColumnSeries
            //{
            //    Title = "Ilość sprzedanych produktów: ",
            //    Values = new ChartValues<int>(productSales.Select(p => (p.Quantity))),
            //    DataLabels = true,
            //});

            //Labels = productSales.Select(p => p.ProductName).ToList();
            //Values = value => value.ToString("N");
        }
    }
}
