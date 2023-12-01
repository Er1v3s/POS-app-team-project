using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Orders
    {
        public int Order_id { get; set; }
        public required int Employee_id { get; set; }
        public required int Product_id { get; set; }
        public DateTime Orider_time { get; set; }
        public required int Quantity { get; set; }
    }
}
