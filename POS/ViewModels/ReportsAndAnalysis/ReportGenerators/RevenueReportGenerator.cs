using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class RevenueReportGenerator : IReportGenerator<RevenueReportDto>
    {
        public async Task<List<RevenueReportDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var orders = await dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments,
                    order => order.OrderId,
                    payment => payment.OrderId,
                    (order, payment) => new RevenueReportDto{ Date = order.OrderTime, TotalRevenue = (float)payment.Amount })
                .ToListAsync();

            IEnumerable<RevenueReportDto> ordersReport;

            switch (groupBy)
            {
                case GroupBy.Day:
                    ordersReport = GroupDataByDays(orders);
                    break;
                case GroupBy.Week:
                    ordersReport = GroupDataByWeeks(orders);
                    break;
                case GroupBy.Month:
                    ordersReport = GroupDataByMonths(orders);
                    break;
                case GroupBy.Year:
                    ordersReport = GroupDataByYears(orders);
                    break;
                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            var orderedData = ordersReport.OrderBy(revenue => revenue.Date).ToList();

            orderedData = CompleteMissingData(orderedData, startDate, endDate, groupBy);

            return orderedData;
        }

        private IEnumerable<RevenueReportDto> GroupDataByDays(List<RevenueReportDto> orders)
        {
            return orders
                .GroupBy(x => new { x.Date.Year, x.Date.Month, x.Date.Day })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                });
        }

        private IEnumerable<RevenueReportDto> GroupDataByWeeks(List<RevenueReportDto> orders)
        {
            return orders
                .GroupBy(x => x.Date.DayOfWeek)
                .Select(g => new RevenueReportDto
                {
                    DayOfWeek = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                })
                .OrderBy(order => order.DayOfWeek);
        }

        private IEnumerable<RevenueReportDto> GroupDataByMonths(List<RevenueReportDto> orders)
        {
            return orders
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                });
        }

        private IEnumerable<RevenueReportDto> GroupDataByYears(List<RevenueReportDto> orders)
        {
            return orders
                .GroupBy(x => x.Date.Year)
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key, 1, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                });
        }

        private List<RevenueReportDto> CompleteMissingData(List<RevenueReportDto> orderedData, DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = orderedData.FirstOrDefault(r => r.Date.Date == date.Date);
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
                        if (orderedData.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            orderedData.Add(new RevenueReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                TotalRevenue = 0f
                            });
                        }
                    }
                    var result = orderedData.OrderBy(data => data.DayOfWeek).ToList();
                    return result;

                default:
                    return orderedData;
            }
        }
    }
}
