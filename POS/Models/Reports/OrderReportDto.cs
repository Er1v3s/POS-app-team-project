using System;

namespace POS.Models.Reports
{
    public class OrderReportDto : IReportDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }

        float IReportDto.Value
        {
            get => OrderCount;
            set => OrderCount = (int)value;
        }
    }
}
