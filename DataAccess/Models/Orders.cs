﻿using System;

namespace DataAccess.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public required int EmployeeId { get; set; }
        public DateTime OrderTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
