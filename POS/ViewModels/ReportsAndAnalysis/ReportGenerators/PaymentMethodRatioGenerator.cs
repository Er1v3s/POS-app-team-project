﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class PaymentMethodRatioGenerator : IReportGenerator<PaymentRatioDto>
    {
        public async Task<IQueryable<PaymentRatioDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            await using var dbContext = new AppDbContext();

            var paymentRatio = dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Payments, order => order.OrderId, payment => payment.OrderId,
                    (order, payment) => payment)
                .GroupBy(payment => payment.PaymentMethod)
                .ToList()
                .Select(group => new PaymentRatioDto
                {
                    PaymentMethod = group.Key,
                    Count = group.Count()
                })
                .AsQueryable();

            return paymentRatio;
        }
    }
}
