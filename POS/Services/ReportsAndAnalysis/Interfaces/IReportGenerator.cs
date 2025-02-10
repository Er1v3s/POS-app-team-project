using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Reports;

namespace POS.Services.ReportsAndAnalysis.Interfaces
{
    public interface IReportGenerator<T>
    {
        Task<List<T>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy = null);
    }
}
