using System.Threading.Tasks;
using LiveCharts;

namespace POS.Services.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionsFactory
    {
        Task GeneratePrediction(int selectedReportIndex, SeriesCollection seriesCollection);
        public object GetPredictionData();
    }
}
