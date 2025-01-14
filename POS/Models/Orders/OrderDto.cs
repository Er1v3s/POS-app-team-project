using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Orders
{
    public class OrderDto
    {
        public int EmployeeId { get; set; }
        public List<OrderItemDto> OrderItemList { get; set; }
        public double AmountToPay { get; set; }
        public string PaymentMethod { get; set; }
    }
}
