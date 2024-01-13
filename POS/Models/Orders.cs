using System;

namespace POS.Models
{
    public class Orders
    {
        public int Order_id { get; set; }
        public required int Employee_id { get; set; }
        public DateTime Order_time { get; set; }
    }
}
