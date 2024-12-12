using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class SalesReportGenerator : IReportGenerator<ProductSalesDto>
    {
        public async Task<IQueryable<ProductSalesDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var orderedItems = dbContext.OrderItems
                .Join(
                    dbContext.Orders.Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate),
                    orderItem => orderItem.OrderId,
                    order => order.OrderId,
                    (orderItem, order) => new { orderItem, order }
                )
                .Join(
                    dbContext.Products,
                    combined => combined.orderItem.ProductId,
                    product => product.ProductId,
                    (combined, product) => new ProductSalesDto
                    {
                        Date = combined.order.OrderTime,
                        ProductName = product.ProductName,
                        Quantity = combined.orderItem.Quantity
                    }
                );

            var orderedItemsConverted = ConvertDate(orderedItems);

            var orderedItemsGrouped = GroupData(orderedItemsConverted, groupBy);

            return orderedItemsGrouped;
        }

        private IQueryable<ProductSalesDto> GroupData(IQueryable<ProductSalesDto> orderedItems, GroupBy? groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(orderedItems);
                case GroupBy.Month:
                    return GroupDataByMonths(orderedItems);
                case GroupBy.Year:
                    return GroupDataByYears(orderedItems);
                default:
                    return orderedItems;
            }
        }

        private IQueryable<ProductSalesDto> GroupDataByDays(IQueryable<ProductSalesDto> orderedItems)
        {
            return orderedItems.ToList()
                .GroupBy(dto => new { dto.ProductName, Date = dto.Date.Date })
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.Key.ProductName,
                    Date = group.Key.Date,
                    Quantity = group.Sum(g => g.Quantity)
                })
                .OrderBy(group => group.Date)
                .AsQueryable();
        }

        private IQueryable<ProductSalesDto> GroupDataByMonths(IQueryable<ProductSalesDto> orderedItems)
        {
            return orderedItems.ToList()
                .GroupBy(dto => new { dto.ProductName, dto.Date.Year, dto.Date.Month })
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.Key.ProductName,
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                    Quantity = group.Sum(g => g.Quantity)
                })
                .OrderBy(group => group.Date)
                .AsQueryable();
        }

        private IQueryable<ProductSalesDto> GroupDataByYears(IQueryable<ProductSalesDto> orderedItems)
        {
            return orderedItems.ToList()
                .GroupBy(dto => new { dto.ProductName, dto.Date.Year })
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.Key.ProductName,
                    Date = new DateTime(group.Key.Year, 1, 1),
                    Quantity = group.Sum(g => g.Quantity)
                })
                .OrderBy(group => group.Date)
                .AsQueryable();
        }


        private IQueryable<ProductSalesDto> ConvertDate(IQueryable<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .Select(group => new ProductSalesDto
                {
                    Date = new DateTime(group.Date.Year, group.Date.Month, group.Date.Day),
                    ProductName = group.ProductName,
                    Quantity = group.Quantity
                });
        }
    }
}
