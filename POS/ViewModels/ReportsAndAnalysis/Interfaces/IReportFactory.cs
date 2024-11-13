using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IReportFactory
    {
        void SetParameters(SeriesCollection seriesCollection, DateTime startDate, DateTime endDate);
        Task GenerateReport(int selectedReportIndex);
        List<string> GetUpdatedLabelsValues();
    }
}
