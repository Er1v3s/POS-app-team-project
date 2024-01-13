using System;

namespace POS.Models
{
    public class Payments
    {
        public int Payment_id { get; set; }
        public int Order_id { get; set; }
        public DateTime Payment_time { get; set; }
        public string Payment_method { get; set; }
        public double Amount { get; set; }
    }
}
