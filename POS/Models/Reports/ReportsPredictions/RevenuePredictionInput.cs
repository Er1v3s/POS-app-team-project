using Microsoft.ML.Data;
using System;

namespace POS.Models.Reports.ReportsPredictions
{
    public class RevenuePredictionInput
    {
        [LoadColumn(0)]
        public DateTime Date { get; set; }

        [LoadColumn(1)]
        public float TotalRevenue { get; set; }
    }
}
