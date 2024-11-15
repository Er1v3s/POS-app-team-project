using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IReportFactory
    {
        void SetParameters(SeriesCollection seriesCollection, DateTime startDate, DateTime endDate);
        Task GenerateReport(int selectedReportIndex);
        List<string> GetUpdatedLabelsValues();
        Task GeneratePrediction(int selectedReportIndex);
    }
}
