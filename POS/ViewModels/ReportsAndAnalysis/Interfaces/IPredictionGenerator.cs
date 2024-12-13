using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<TInput, TOutput>
    {
        Task<List<TOutput>> GeneratePrediction(List<TInput> data, int windowSize, int horizon, GroupBy groupBy);
    }
}
