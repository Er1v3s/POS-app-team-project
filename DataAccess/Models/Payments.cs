﻿using System;

namespace DataAccess.Models
{
    public class Payments
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public double Amount { get; set; }
    }
}
