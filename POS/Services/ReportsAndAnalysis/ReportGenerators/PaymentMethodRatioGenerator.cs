using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.Services.ReportsAndAnalysis.Interfaces;

namespace POS.Services.ReportsAndAnalysis.ReportGenerators
{
    public class PaymentMethodRatioGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<PaymentRatioDto>
    {
        public async Task<List<PaymentRatioDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {

            var paymentRatio = await _dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(_dbContext.Payments, order => order.OrderId, payment => payment.OrderId,
                    (order, payment) => payment)
                .GroupBy(payment => payment.PaymentMethod)
                .Select(group => new PaymentRatioDto
                {
                    PaymentMethod = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return paymentRatio;
        }
    }
}
