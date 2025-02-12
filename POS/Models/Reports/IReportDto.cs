using System;

namespace POS.Models.Reports
{
    public interface IReportDto
    {
        DateTime Date { get; set; }
        float Value { get; set; }
    }
}
