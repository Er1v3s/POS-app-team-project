using System;

namespace POS.Models.Reports
{
    public class ProductSalesDto : IReportDto
    {
        public string ProductName { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

        float IReportDto.Value
        {
            get => Quantity;
            set => Quantity = (int)value;
        }
    }
}
