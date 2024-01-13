﻿using System;

namespace POS.Models
{
    public class OrderItems
    {
        public int OrderItem_id { get; set; }
        public required int OrdersOrder_id { get; set; }
        public required int Product_id { get; set; }
        public required int Quantity { get; set; }
        public required int Employee_id { get; set; }
        public required DateTime Orider_time { get; set; }
    }
}
