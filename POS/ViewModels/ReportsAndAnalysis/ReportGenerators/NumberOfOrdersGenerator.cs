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
    public class NumberOfOrdersGenerator : IReportGenerator<OrderReportDto>
    {
        public async Task<List<OrderReportDto>> GenerateData(DateTime startDate, DateTime endDate, string? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var orders = await dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .ToListAsync();

            IEnumerable<OrderReportDto> ordersReport;

            switch (groupBy)
            {
                case "day":
                    ordersReport = orders
                        .GroupBy(order => order.OrderTime.Date)
                        .Select(group => new OrderReportDto
                        {
                            Date = group.Key,
                            OrderCount = group.Count()
                        });
                    break;
                case "week":
                    ordersReport = orders.GroupBy(order => order.DayOfWeek)
                        .Select(group => new OrderReportDto
                        {
                            DayOfWeek = group.Key,
                            OrderCount = group.Count()
                        })
                        .OrderBy(order => order.DayOfWeek);
                    break;
                case "month":
                    ordersReport = orders
                        .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                            OrderCount = group.Count()
                        });
                    break;

                case "year":
                    ordersReport = orders
                        .GroupBy(order => order.OrderTime.Year)
                        .Select(group => new OrderReportDto
                        {
                            Date = new DateTime(group.Key, 1, 1),
                            OrderCount = group.Count()
                        });
                    break;

                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            var orderedData = ordersReport.OrderBy(revenue => revenue.Date).ToList();

            orderedData = CompleteMissingData(orderedData, startDate, endDate, groupBy);

            return orderedData;
        }

        private List<OrderReportDto> CompleteMissingData(List<OrderReportDto> orderedData, DateTime startDate,
            DateTime endDate, string groupBy)
        {
            switch (groupBy)
            {
                case "day":
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = orderedData.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new OrderReportDto
                        {
                            Date = date,
                            OrderCount = 0,
                        };
                    }).ToList();

                case "week":
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (orderedData.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            orderedData.Add(new OrderReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                OrderCount = 0,
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
