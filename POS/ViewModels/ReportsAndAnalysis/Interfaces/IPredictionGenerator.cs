using POS.Models.Reports.ReportsPredictions;
using System.Collections.Generic;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<TInput, TOutput>
    {
        List<TOutput> GeneratePrediction(List<TInput> data, int windowSize, int seriesLength, int horizon);
    }
}
