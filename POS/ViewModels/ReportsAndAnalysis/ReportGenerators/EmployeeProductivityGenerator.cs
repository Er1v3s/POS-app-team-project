using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public class EmployeeProductivityGenerator(AppDbContext dbContext) : ReportGenerator(dbContext), IReportGenerator<EmployeeProductivityDto>
    {
        public async Task<IQueryable<EmployeeProductivityDto>> GenerateData(DateTime startDate, DateTime endDate, GroupBy? groupBy)
        {
            var productivity = _dbContext.Orders
                .Where(order => order.OrderTime >= startDate && order.OrderTime <= endDate)
                .Join(_dbContext.Employees, order => order.EmployeeId, employee => employee.EmployeeId,
                    (order, employee) => new { order, employee })
                .Join(_dbContext.Payments, orderEmployee => orderEmployee.order.OrderId, payment => payment.OrderId,
                    (orderEmployee, payment) => new { orderEmployee.order, orderEmployee.employee, payment })
                .GroupBy(x => new { x.employee.EmployeeId, x.employee.FirstName, x.employee.LastName })
                .ToList()
                .Select(g => new EmployeeProductivityDto
                {
                    EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                    OrderCount = g.Count(),
                    TotalAmount = Math.Round(g.Sum(x => x.payment.Amount), 2)
                }).AsQueryable();

            return productivity;
        }
    }
}
