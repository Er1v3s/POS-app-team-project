using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using POS.Models.Orders;

namespace POS.Services.SalesPanel
{
    public class FinishedOrderService
    {
        private readonly AppDbContext _dbContext;

        public FinishedOrderService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<OrderHistoryDto>> GetFinishedOrders()
        {
            List<OrderHistoryDto> finishedOrders = new();

            var orders = await _dbContext.Orders.ToListAsync();

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == order.EmployeeId);
                    var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

                    OrderHistoryDto formattedOrder = new()
                    {
                        OrderId = order.OrderId,
                        EmployeeName = employee.FirstName + " " + employee.LastName,
                        OrderDate = order.OrderTime.ToString("dd/MM/yyyy"),
                        OrderTime = order.OrderTime.ToString("HH:mm"),
                        AmountToPay = payment.Amount,
                        PaymentMethod = payment.PaymentMethod,
                    };

                    finishedOrders.Add(formattedOrder);
                }
            }

            return finishedOrders;
        }
    }
}
