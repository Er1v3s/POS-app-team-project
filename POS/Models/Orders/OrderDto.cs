using System.Collections.Generic;
using POS.Models.Invoices;

namespace POS.Models.Orders
{
    public class OrderDto
    {
        public int EmployeeId { get; set; }
        public List<OrderItemDto> OrderItemList { get; set; }
        public double AmountToPay { get; set; }
        public string PaymentMethod { get; set; }
        public int Discount { get; set; }
        public InvoiceDto? InvoiceData { get; set; }
    }
}
