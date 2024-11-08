using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IReportGenerator<T>
    {
        Task<List<T>> GenerateData(DateTime startDate, DateTime endDate, string? groupBy = null);
    }
}
