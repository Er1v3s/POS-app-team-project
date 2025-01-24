using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class SalesReportGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<ProductSalesDto>
    {
        public async Task<List<ProductSalesDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            var orderedItems = _dbContext.OrderItems
                .Join(
                    _dbContext.Orders.Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate),
                    orderItem => orderItem.OrderId,
                    order => order.OrderId,
                    (orderItem, order) => new { orderItem, order }
                )
                .Join(
                    _dbContext.Product,
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

            var orderedItemsGrouped = await GroupData(orderedItemsConverted, groupBy);

            return orderedItemsGrouped;
        }

        private async Task<List<ProductSalesDto>> GroupData(IQueryable<ProductSalesDto> orderedItems, GroupBy? groupBy)
        {
            var orderedItemsList = await orderedItems.ToListAsync();

            switch (groupBy)
            {
                case GroupBy.Day:
                    return GroupDataByDays(orderedItemsList);
                case GroupBy.Month:
                    return GroupDataByMonths(orderedItemsList);
                case GroupBy.Year:
                    return GroupDataByYears(orderedItemsList);
                default:
                    return orderedItemsList;
            }
        }

        private List<ProductSalesDto> GroupDataByDays(List<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .GroupBy(dto => new { dto.ProductName, Date = dto.Date.Date })
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.Key.ProductName,
                    Date = group.Key.Date,
                    Quantity = group.Sum(g => g.Quantity)
                })
                .OrderBy(group => group.Date)
                .ToList();
        }

        private List<ProductSalesDto> GroupDataByMonths(List<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .GroupBy(dto => new { dto.ProductName, dto.Date.Year, dto.Date.Month })
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.Key.ProductName,
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                    Quantity = group.Sum(g => g.Quantity)
                })
                .OrderBy(group => group.Date)
                .ToList();
        }

        private List<ProductSalesDto> GroupDataByYears(List<ProductSalesDto> orderedItems)
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
                .ToList();
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
