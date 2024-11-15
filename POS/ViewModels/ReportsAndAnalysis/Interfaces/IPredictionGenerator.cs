using POS.Models.Reports.ReportsPredictions;
using System.Collections.Generic;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    public interface IPredictionGenerator<T> // Zmiana parametru generycznego
    {
        List<RevenuePredictionDto> GeneratePrediction(List<T> data);
    }
}
