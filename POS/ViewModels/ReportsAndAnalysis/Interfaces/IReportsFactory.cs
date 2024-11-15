using System;
using System.Threading.Tasks;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IReportsFactory
    {
        void SetParameters(DateTime startDate, DateTime endDate);
        Task GenerateReport(int selectedReportIndex);
        object GetReportData();
    }
}
