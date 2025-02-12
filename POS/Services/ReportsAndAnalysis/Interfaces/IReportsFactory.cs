using System;
using System.Threading.Tasks;

namespace POS.Services.ReportsAndAnalysis.Interfaces
{
    public interface IReportsFactory
    {
        void SetParameters(DateTime startDate, DateTime endDate);
        Task GenerateReport(int selectedReportIndex);
        object GetReportData();
    }
}
