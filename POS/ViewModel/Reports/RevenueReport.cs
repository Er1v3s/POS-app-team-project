using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModel.Reports
{
    public class RevenueReport
    {
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
