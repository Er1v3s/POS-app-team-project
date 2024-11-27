using Microsoft.ML.Data;

namespace POS.Models.Reports.ReportsPredictions
{
    public class PredictionDataModel
    {
        [VectorType(7)] // The forecast horizon (e.g., predicting for the next 7 days)
        public float[] Total { get; set; }
    }
}
