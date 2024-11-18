using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class RevenueReportGenerator : IReportGenerator<RevenueReportDto>
    {
        IQueryable<RevenueReportDto> groupedQuery;

        public async Task<List<RevenueReportDto>> GenerateData(DateTime startDate, DateTime endDate, string? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var revenueReportQuery = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new { order.OrderTime, payment.Amount })
                .AsEnumerable();

            switch (groupBy)
            {
                case "day":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Date)
                        .Select(g => new RevenueReportDto
                        {
                            Date = g.Key,
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;
                case "week":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.DayOfWeek)
                        .Select(g => new RevenueReportDto
                        {
                            DayOfWeek = g.Key,
                            TotalRevenue = (float)g.Sum(x => x.Amount),
                        })
                        .OrderBy(order => order.DayOfWeek)
                        .AsQueryable();
                    break;
                case "month":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => new { x.OrderTime.Year, x.OrderTime.Month })
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;
                case "year":
                    groupedQuery = revenueReportQuery
                        .GroupBy(x => x.OrderTime.Year)
                        .Select(g => new RevenueReportDto
                        {
                            Date = new DateTime(g.Key, 1, 1),
                            TotalRevenue = (float)g.Sum(x => x.Amount)
                        })
                        .AsQueryable();
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            var groupedData = groupedQuery.OrderBy(revenue => revenue.Date).ToList();

            groupedData = CompleteMissingData(groupedData, startDate, endDate, groupBy);

            return groupedData;
        }

        private List<RevenueReportDto> CompleteMissingData(List<RevenueReportDto> groupedData, DateTime startDate, DateTime endDate, string groupBy)
        {
            switch (groupBy)
            {
                case "day":
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = groupedData.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new RevenueReportDto
                        {
                            Date = date,
                            TotalRevenue = 0f
                        };
                    }).ToList();

                case "week":
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (groupedData.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            groupedData.Add(new RevenueReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                TotalRevenue = 0f
                            });
                        }
                    }
                    var result = groupedData.OrderBy(data => data.DayOfWeek).ToList();
                    return result;

                default:
                    return groupedData;
            }
        }
    }
}
