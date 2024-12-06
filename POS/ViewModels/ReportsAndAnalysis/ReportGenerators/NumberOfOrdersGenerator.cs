using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class NumberOfOrdersGenerator : IReportGenerator<OrderReportDto>
    {
        public async Task<IQueryable<OrderReportDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy = null)
        {
            await using var dbContext = new AppDbContext();

            var orders = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate);

            var ordersGrouped = GroupData(orders, groupBy);

            var ordersGroupedWithMissingData = CompleteMissingData(ordersGrouped, startDate, endDate, groupBy);

            return ordersGroupedWithMissingData.AsQueryable();
        }

        private IQueryable<OrderReportDto> GroupData(IQueryable<Orders> orders, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(orders);
                case GroupBy.Week:
                    return GroupDataByWeeks(orders);
                case GroupBy.Month:
                    return GroupDataByMonths(orders);
                case GroupBy.Year:
                    return GroupDataByYears(orders);
                default:
                    throw new Exception("Invalid groupBy argument");
            }
        }

        private IQueryable<OrderReportDto> GroupDataByDays(IQueryable<Orders> orders)
        {
            return orders
                .GroupBy(order => order.OrderTime.Date)
                .Select(group => new OrderReportDto
                {
                    Date = group.Key,
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .AsQueryable();
        }

        private IQueryable<OrderReportDto> GroupDataByWeeks(IQueryable<Orders> orders)
        {
            return orders.GroupBy(order => order.DayOfWeek)
                .Select(group => new OrderReportDto
                {
                    DayOfWeek = group.Key,
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.DayOfWeek)
                .AsQueryable();
        }

        private IQueryable<OrderReportDto> GroupDataByMonths(IQueryable<Orders> orders)
        {
            return orders.ToList()
                .GroupBy(order => new { order.OrderTime.Year, order.OrderTime.Month })
                .Select(group => new OrderReportDto
                {
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .AsQueryable();
        }

        private IQueryable<OrderReportDto> GroupDataByYears(IQueryable<Orders> orders)
        {
            return orders.ToList()
                .GroupBy(order => order.OrderTime.Year)
                .Select(group => new OrderReportDto
                {
                    Date = new DateTime(group.Key, 1, 1),
                    OrderCount = group.Count()
                })
                .OrderBy(order => order.Date)
                .AsQueryable();
        }

        private IQueryable<OrderReportDto> CompleteMissingData(IQueryable<OrderReportDto> orders, DateTime startDate,
            DateTime endDate, GroupBy? groupBy)
        {
            var ordersList = orders.ToList();

            switch (groupBy)
            {
                case GroupBy.Day:
                    var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(offset => startDate.AddDays(offset))
                        .ToList();

                    return allDates.Select(date =>
                    {
                        var report = ordersList.FirstOrDefault(r => r.Date.Date == date.Date);
                        return report ?? new OrderReportDto
                        {
                            Date = date,
                            OrderCount = 0,
                        };
                    }).AsQueryable();

                case GroupBy.Week:
                    var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var dayOfWeek in allDaysOfWeek)
                    {
                        if (ordersList.All(data => data.DayOfWeek != dayOfWeek))
                        {
                            ordersList.Add(new OrderReportDto
                            {
                                DayOfWeek = dayOfWeek,
                                OrderCount = 0,
                            });
                        }
                    }

                    var result = ordersList.OrderBy(data => data.DayOfWeek).AsQueryable();
                    return result;

                default:
                    return orders;
            }
        }
    }
}
