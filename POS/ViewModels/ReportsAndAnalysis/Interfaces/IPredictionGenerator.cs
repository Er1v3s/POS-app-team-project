using POS.Models.Reports.ReportsPredictions;
using System.Collections.Generic;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<T>
    {
        List<RevenuePredictionDto> GeneratePrediction(List<T> data);
    }
}
