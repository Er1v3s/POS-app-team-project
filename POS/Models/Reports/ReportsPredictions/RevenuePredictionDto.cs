using Microsoft.ML.Data;
using System;

namespace POS.Models.Reports.ReportsPredictions
{
    public class RevenuePredictionDto
    {
        public DateTime Date { get; set; }
        public float TotalRevenue { get; set; }
    }
}
