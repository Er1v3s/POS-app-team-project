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
    public class SalesReportGenerator : IReportGenerator<ProductSalesDto>
    {
        public async Task<List<ProductSalesDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var orderedItems = await dbContext.OrderItems
                .Join(
                    dbContext.Orders,
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
                )
                .Where(data => data.Date >= startDate && data.Date <= endDate)
                .ToListAsync();


            var productSalesConverted = ConvertDate(orderedItems);
            var productSalesGrouped = GroupDataByProductNames(productSalesConverted);
            var orderedData = productSalesGrouped.OrderBy(sales => sales.Date).ToList();

            return orderedData;
        }

        private IEnumerable<ProductSalesDto> GroupDataByProductNames(IEnumerable<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .GroupBy(item => item.ProductName )
                .Select(group => new ProductSalesDto
                {
                    ProductName = group.First().ProductName,
                    Quantity = group.Sum(item => item.Quantity)
                });
        }

        private IEnumerable<ProductSalesDto> ConvertDate(List<ProductSalesDto> orderedItems)
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
