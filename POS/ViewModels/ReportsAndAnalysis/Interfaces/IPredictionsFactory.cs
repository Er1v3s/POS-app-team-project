using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using POS.Models.Reports.ReportsPredictions;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionsFactory
    {
        Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection);
        List<RevenuePredictionDto> GetPredictionData();
    }
}
