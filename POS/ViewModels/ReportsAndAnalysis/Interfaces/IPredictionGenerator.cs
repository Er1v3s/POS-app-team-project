using System.Collections.Generic;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<TInput, TOutput>
    {
        List<TOutput> GeneratePrediction(List<TInput> data, int windowSize, int seriesLength, int horizon, GroupBy groupBy);
    }
}
