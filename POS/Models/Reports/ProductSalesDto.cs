using System;

namespace POS.Models.Reports
{
    public class ProductSalesDto
    {
        public string ProductName { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
