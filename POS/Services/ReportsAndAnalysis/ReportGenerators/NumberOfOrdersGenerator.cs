using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.Services.ReportsAndAnalysis.Interfaces;

namespace POS.Services.ReportsAndAnalysis.ReportGenerators
{
    public class NumberOfOrdersGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<OrderReportDto>
    {
        public async Task<List<OrderReportDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy = null)
        {
            var orders = _dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate);

            var ordersGrouped = await GroupData(orders, groupBy);

            var ordersGroupedWithMissingData = CompleteMissingData(ordersGrouped, startDate, endDate, groupBy);

            return ordersGroupedWithMissingData;
        }

        private async Task<List<OrderReportDto>> GroupData(IQueryable<Order> orders, GroupBy? groupBy)
        {
            var ordersList = await orders.ToListAsync();

            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(ordersList);
                case GroupBy.Week:
                    return GroupDataByWeeks(ordersList);
                case GroupBy.Month:
                    return GroupDataByMonths(ordersList);
                case GroupBy.Year:
                    return GroupDataByYears(ordersList);
                default:
                    throw new Exception("Invalid groupBy argument");
            }
        }

        private List<OrderReportDto> GroupDataByDays(List<Order> orders)
        {
            return orders
                .GroupBy(order => order.OrderTime.Date)
                .Select(group => new OrderReportDto
                {
                    Date = group.Key,
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .ToList();
        }

        private List<OrderReportDto> GroupDataByWeeks(List<Order> orders)
        {
            return orders.GroupBy(order => order.DayOfWeek)
                .Select(group => new OrderReportDto
                {
                    DayOfWeek = group.Key,
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.DayOfWeek)
                .ToList();
        }

        private List<OrderReportDto> GroupDataByMonths(List<Order> orders)
        {
            return orders.ToList()
                .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                .Select(group => new OrderReportDto
                {
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .ToList();
        }

        private List<OrderReportDto> GroupDataByYears(List<Order> orders)
        {
            return orders.ToList()
                .GroupBy(order => order.OrderTime.Year)
                .Select(group => new OrderReportDto
                {
                    Date = new DateTime(group.Key, 1, 1),
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .ToList();
        }

        private List<OrderReportDto> CompleteMissingData(List<OrderReportDto> orders, DateTime startDate,
            DateTime endDate, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = orders.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new OrderReportDto
                        {
                            Date = date,
                            OrderCount = 0,
                        };
                    }).ToList();

                case GroupBy.Week:
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (orders.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            orders.Add(new OrderReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                OrderCount = 0,
                            });
                        }
                    }

                    var result = orders.OrderBy(data => data.DayOfWeek).ToList();
                    return result;

                default:
                    return orders;
            }
        }
    }
}
