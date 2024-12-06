using System;
using System.Linq;
using System.Threading.Tasks;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IReportGenerator<T>
    {
        Task<IQueryable<T>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy = null);
    }
}
