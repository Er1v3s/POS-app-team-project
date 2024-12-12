using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class RevenueReportGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<RevenueReportDto>
    {
        public async Task<IQueryable<RevenueReportDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
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
            
            var revenueGrouped = GroupData(revenue, groupBy);

            var revenueGroupedWithMissingData = CompleteMissingData(revenueGrouped, startDate, endDate, groupBy);

            return revenueGroupedWithMissingData;
        }

        private IQueryable<RevenueReportDto> GroupData(IQueryable<RevenueReportDto> revenue, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(revenue);
                case GroupBy.Week:
                    return GroupDataByWeeks(revenue);
                case GroupBy.Month:
                    return GroupDataByMonths(revenue);
                case GroupBy.Year:
                    return GroupDataByYears(revenue);
                default:
                    throw new Exception("Invalid groupBy argument");
            }
        }

        private IQueryable<RevenueReportDto> GroupDataByDays(IQueryable<RevenueReportDto> revenue)
        {
            return revenue.ToList()
                .GroupBy(x => new { x.Date.Year, x.Date.Month, x.Date.Day })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .AsQueryable();
        }

        private IQueryable<RevenueReportDto> GroupDataByWeeks(IQueryable<RevenueReportDto> revenue)
        {
            return revenue.ToList()
                .GroupBy(x => x.Date.DayOfWeek)
                .Select(g => new RevenueReportDto
                {
                    DayOfWeek = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                })
                .OrderBy(order => order.DayOfWeek)
                .AsQueryable();
        }

        private IQueryable<RevenueReportDto> GroupDataByMonths(IQueryable<RevenueReportDto> revenue)
        {
            return revenue.ToList()
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .AsQueryable();
        }

        private IQueryable<RevenueReportDto> GroupDataByYears(IQueryable<RevenueReportDto> revenue)
        {
            return revenue.ToList()
                .GroupBy(x => x.Date.Year)
                .Select(g => new RevenueReportDto
                {
                    Date = new DateTime(g.Key, 1, 1),
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .AsQueryable();
        }

        private IQueryable<RevenueReportDto> CompleteMissingData(IQueryable<RevenueReportDto> revenue, DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            var revenueList = revenue.ToList();

            switch (groupBy)
            {
                case GroupBy.Day:
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = revenueList.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new RevenueReportDto
                        {
                            Date = date,
                            TotalRevenue = 0f
                        };
                    }).AsQueryable();

                case GroupBy.Week:
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (revenueList.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            revenueList.Add(new RevenueReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                TotalRevenue = 0f
                            });
                        }
                    }
                    var result = revenueList.OrderBy(data => data.DayOfWeek).AsQueryable();
                    return result;

                default:
                    return revenue;
            }
        }
    }
}
