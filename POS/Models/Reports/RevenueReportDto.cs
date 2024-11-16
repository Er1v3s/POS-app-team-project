using System;

namespace POS.Models.Reports
{
    public class RevenueReportDto
    {
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public float TotalRevenue { get; set; }
    }
}
