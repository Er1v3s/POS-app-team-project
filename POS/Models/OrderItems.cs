using System;

namespace POS.Models
{
    public class OrderItems
    {
        public int OrderItemId { get; set; }
        public required int OrderId { get; set; }
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public required int EmployeeId { get; set; }
    }
}
