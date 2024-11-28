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
    public class CardToCashPaymentRatioGenerator : IReportGenerator<PaymentRatioDto>
    {
        public async Task<List<PaymentRatioDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var paymentRatio = await dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments, order => order.OrderId, payment => payment.OrderId,
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
