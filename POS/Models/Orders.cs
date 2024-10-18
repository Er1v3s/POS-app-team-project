using System;

namespace POS.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public required int EmployeeId { get; set; }
        public DateTime OrderTime { get; set; }
    }
}
