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
            //var productSalesGrouped = GroupDataByProductNames(productSalesConverted);
            //var orderedData = productSalesConverted.OrderBy(sales => sales.Date).ToList();

            var ordersReport = new List<ProductSalesDto>();

            switch (groupBy)
            {
                case GroupBy.Day:
                    ordersReport = GroupDataByDays(productSalesConverted);
                    break;
                //case GroupBy.Week:
                //    ordersReport = GroupDataByWeeks(data);
                //    break;
                case GroupBy.Month:
                    ordersReport = GroupDataByMonths(productSalesConverted);
                    break;
                case GroupBy.Year:
                    ordersReport = GroupDataByYears(productSalesConverted);
                    break;
                default:
                    throw new ArgumentException("Invalid groupBy value");
            }

            return ordersReport;
        }

        private List<ProductSalesDto> GroupDataByDays(List<ProductSalesDto> data)
        {
            return data
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

        //private List<ProductSalesDto> GroupDataByWeeks(List<ProductSalesDto> data)
        //{
        //    var groupedData = data
        //        .GroupBy(dto => new { dto.ProductName, Date = dto.Date.Date })
        //        .Select(group => new ProductSalesDto
        //        {
        //            ProductName = group.Key.ProductName,
        //            Date = group.Key.Date,
        //            Quantity = group.Sum(g => g.Quantity)
        //        })
        //        .OrderBy(dto => dto.Date)
        //        .ToList();

        //    return groupedData;
        //}

        private List<ProductSalesDto> GroupDataByMonths(List<ProductSalesDto> data)
        {
            return data
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

        private List<ProductSalesDto> GroupDataByYears(List<ProductSalesDto> data)
        {
            return data
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


        private List<ProductSalesDto> ConvertDate(List<ProductSalesDto> orderedItems)
        {
            return orderedItems
                .Select(group => new ProductSalesDto
                {
                    Date = new DateTime(group.Date.Year, group.Date.Month, group.Date.Day),
                    ProductName = group.ProductName,
                    Quantity = group.Quantity
                }).ToList();
        }
    }
}
