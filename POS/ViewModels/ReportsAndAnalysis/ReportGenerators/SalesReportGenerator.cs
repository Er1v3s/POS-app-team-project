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
        public async Task<List<ProductSalesDto>> GenerateData(DateTime startDate, DateTime endDate, string? orderBy = null)
        {
            await using var dbContext = new AppDbContext();

            var productSales = await dbContext.OrderItems
                .Where(orderItem => dbContext.Orders
                    .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                    .Select(order => order.OrderId)
                    .Contains(orderItem.OrderId))
                .GroupBy(orderItem => orderItem.ProductId)
                .Select(groupedItems => new ProductSalesDto
                {
                    ProductName = dbContext.Products
                        .Where(product => product.ProductId == groupedItems.Key)
                        .Select(product => product.ProductName)
                        .FirstOrDefault(),
                    Quantity = groupedItems.Sum(item => item.Quantity)
                })
                .ToListAsync();

            return productSales;
        }
    }
}
