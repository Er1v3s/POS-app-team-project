using System;

namespace POS.Models.Reports
{
    public class RevenueReportDto : IReportDto
    {
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public float TotalRevenue { get; set; }

        float IReportDto.Value
        {
            get => TotalRevenue;
            set => TotalRevenue = value;
        }
    }
}
