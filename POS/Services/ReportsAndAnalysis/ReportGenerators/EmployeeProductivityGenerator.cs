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
    public class EmployeeProductivityGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<EmployeeProductivityDto>
    {
        public async Task<List<EmployeeProductivityDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            var productivity = await _dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(_dbContext.Employees, order => order.EmployeeId, employee => employee.EmployeeId,
                    (order, employee) => new { order, employee })
                .Join(_dbContext.Payments, orderEmployee => orderEmployee.order.OrderId, payment => payment.OrderId,
                    (orderEmployee, payment) => new { orderEmployee.order, orderEmployee.employee, payment })
                .GroupBy(x => new { x.employee.EmployeeId, x.employee.FirstName, x.employee.LastName })
                .Select(g => new EmployeeProductivityDto
                {
                    EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                    OrderCount = g.Count(),
                    TotalAmount = Math.Round(g.Sum(x => x.payment.Amount), 2)
                }).ToListAsync();

            return productivity;
        }
    }
}
