using System;

namespace POS.Models.Reports
{
    public class OrderReportDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
    }
}
