using System.Collections.Generic;

namespace POS.Models.Orders
{
    public class OrderDto
    {
        public int EmployeeId { get; set; }
        public List<OrderItemDto> OrderItemList { get; set; }
        public double AmountToPay { get; set; }
        public string PaymentMethod { get; set; }
        public int Discount { get; set; }
    }
}
