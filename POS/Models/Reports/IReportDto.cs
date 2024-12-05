using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Reports
{
    public interface IReportDto
    {
        DateTime Date { get; set; }
        float Value { get; set; }
    }
}
