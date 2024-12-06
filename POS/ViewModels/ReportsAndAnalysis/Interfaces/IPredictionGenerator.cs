using System.Linq;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<in TInput, out TOutput>
    {
        IQueryable<TOutput> GeneratePrediction(IQueryable<TInput> data, int windowSize, int horizon, GroupBy groupBy);
    }
}
