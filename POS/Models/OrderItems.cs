using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class OrderItems
    {
        public int OrderItem_id { get; set; }
        public required int Order_id { get; set; }
        public required int Product_id { get; set; }
        public required int Quantity { get; set; }
    }
}
