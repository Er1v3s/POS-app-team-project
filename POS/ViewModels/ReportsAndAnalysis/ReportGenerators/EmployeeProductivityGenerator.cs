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
    public class EmployeeProductivityGenerator : IReportGenerator<EmployeeProductivityDto>
    {
        public async Task<List<EmployeeProductivityDto>> GenerateData(DateTime startDate, DateTime endDate, string? groupBy = null)
        {
            await using var dbContext = new AppDbContext();

            var productivityData = await dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(dbContext.Employees, order => order.EmployeeId, employee => employee.EmployeeId,
                    (order, employee) => new { order, employee })
                .Join(dbContext.Payments, orderEmployee => orderEmployee.order.OrderId, payment => payment.OrderId,
                    (orderEmployee, payment) => new { orderEmployee.order, orderEmployee.employee, payment })
                .GroupBy(x => new { x.employee.EmployeeId, x.employee.FirstName, x.employee.LastName })
                .Select(g => new EmployeeProductivityDto
                {
                    EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                    OrderCount = g.Count(),
                    TotalAmount = Math.Round(g.Sum(x => x.payment.Amount), 2)
                })
                .ToListAsync();

            return productivityData;
        }
    }
}
