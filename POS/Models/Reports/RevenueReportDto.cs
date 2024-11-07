using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Reports
{
    public class RevenueReportDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float TotalRevenue { get; set; }
    }
}
