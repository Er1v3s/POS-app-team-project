using System;

namespace POS.Models.Reports.ReportsPredictions
{
    public class ProductSalesPredictionDto
    {
        public string ProductName { get; set; }
        public DateTime PredictedDate { get; set; }
        public float PredictedQuantity { get; set; }
    }

}
