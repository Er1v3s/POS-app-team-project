﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models.Reports.ReportsPredictions;

namespace POS.Models.Reports
{
    public class RevenueReportDto
    {
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public float TotalRevenue { get; set; }
    }
}
