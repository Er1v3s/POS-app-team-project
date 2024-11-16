using Microsoft.ML.Data;

namespace POS.Models.Reports.ReportsPredictions
{
    public class ProductSalesPredictionInput
    {
        [LoadColumn(0)]
        public string ProductName { get; set; }

        [LoadColumn(1)]
        public float Quantity { get; set; }
    }
}
