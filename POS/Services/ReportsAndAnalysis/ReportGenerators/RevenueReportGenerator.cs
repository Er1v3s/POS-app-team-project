using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.Services.ReportsAndAnalysis.Interfaces;

namespace POS.Services.ReportsAndAnalysis.ReportGenerators
{
    public class RevenueReportGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<RevenueReportDto>
    {
        public async Task<List<RevenueReportDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            var revenue = _dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(_dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new RevenueReportDto
                        { Date = order.OrderTime, TotalRevenue = (float)payment.Amount })
                .OrderBy(revenue => revenue.Date)
                .AsQueryable();
            
            var revenueGrouped = await GroupData(revenue, groupBy);

            var revenueGroupedWithMissingData = CompleteMissingData(revenueGrouped, startDate, endDate, groupBy);

            return revenueGroupedWithMissingData;
        }

        private async Task<List<RevenueReportDto>> GroupData(IQueryable<RevenueReportDto> revenue, GroupBy? groupBy)
        {
            var revenueList = await revenue.ToListAsync();

            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(revenueList);
                case GroupBy.Week:
                    return GroupDataByWeeks(revenueList);
                case GroupBy.Month:
                    return GroupDataByMonths(revenueList);
                case GroupBy.Year:
                    return GroupDataByYears(revenueList);
                default:
                    throw new Exception("Invalid groupBy argument");
            }
        }

        private List<RevenueReportDto> GroupDataByDays(List<RevenueReportDto> revenue)
        {
            return revenue
                .GroupBy(x => new { x.Date.Year, x.Date.Month, x.Date.Day })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .ToList();
        }

        private List<RevenueReportDto> GroupDataByWeeks(List<RevenueReportDto> revenue)
        {
            return revenue
                .GroupBy(x => x.Date.DayOfWeek)
                .Select(g => new RevenueReportDto
                {
                    DayOfWeek = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                })
                .OrderBy(order => order.DayOfWeek)
                .ToList();
        }

        private List<RevenueReportDto> GroupDataByMonths(List<RevenueReportDto> revenue)
        {
            return revenue
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .ToList();
        }

        private List<RevenueReportDto> GroupDataByYears(List<RevenueReportDto> revenue)
        {
            return revenue.ToList()
                .GroupBy(x => x.Date.Year)
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key, 1, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .ToList();
        }

        private List<RevenueReportDto> CompleteMissingData(List<RevenueReportDto> revenue, DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = revenue.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new RevenueReportDto
                        {
                            Date = date,
                            TotalRevenue = 0f
                        };
                    }).ToList();

                case GroupBy.Week:
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (revenue.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            revenue.Add(new RevenueReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                TotalRevenue = 0f
                            });
                        }
                    }
                    var result = revenue.OrderBy(data => data.DayOfWeek).ToList();
                    return result;

                default:
                    return revenue;
            }
        }
    }
}
