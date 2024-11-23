using Microsoft.ML.Data;

namespace POS.Models.Reports.ReportsPredictions
{
    public class RevenuePredictionDataModel
    {
        [VectorType(7)] // The forecast horizon (e.g., predicting for the next 7 days)
        public float[] PredictedRevenue { get; set; }
    }
}
