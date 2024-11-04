using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Reports
{
    public class OrderReportDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
    }
}
