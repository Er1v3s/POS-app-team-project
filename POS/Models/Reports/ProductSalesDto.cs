using System;

namespace POS.Models.Reports
{
    public class ProductSalesDto
    {
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
