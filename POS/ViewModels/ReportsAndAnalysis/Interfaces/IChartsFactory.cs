using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IChartsFactory
    {
        List<string> GetUpdatedLabelsValues();
        Task GenerateChart(int selectedReportIndex, SeriesCollection seriesCollection, ChartType chartType);
    }
}
