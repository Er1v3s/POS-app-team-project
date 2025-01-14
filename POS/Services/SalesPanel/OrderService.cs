using DataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;
using POS.Models.Orders;
using POS.Views.Windows.SalesPanel;

namespace POS.Services.SalesPanel
{
    public class OrderService
    {
        private readonly AppDbContext _dbContext;

        public OrderService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HandleTheOrder(OrderDto orderDto)
        {
            var summaryOrderWindow = new OrderSummaryWindow(orderDto.OrderItemList);
            summaryOrderWindow.ShowDialog();

            if (summaryOrderWindow.DialogResult == true)
            {
                await SaveHandledOrderInDb(orderDto);
                return true;
            }
            else
                return false;
        }

        private async Task SaveHandledOrderInDb(OrderDto orderDto)
        {
            var order = CreateOrder(orderDto);
            var orderEntityEntry = await _dbContext.Orders.AddAsync(order); // because the order id is assigned by the database
            await _dbContext.SaveChangesAsync();

            var orderItemsList = CreateOrderItemList(orderEntityEntry.Entity, orderDto);
            await _dbContext.OrderItems.AddRangeAsync(orderItemsList);

            var payment = CreatePayment(orderEntityEntry.Entity, orderDto);
            await _dbContext.Payments.AddAsync(payment);

            await _dbContext.SaveChangesAsync();
        }

        private Orders CreateOrder(OrderDto orderDto)
        {
            return new Orders
            {
                EmployeeId = orderDto.EmployeeId,
                OrderTime = DateTime.Now,
                DayOfWeek = DateTime.Now.DayOfWeek
            };
        }

        private List<OrderItems> CreateOrderItemList(Orders orderEntityEntry, OrderDto orderDto)
        {
            var orderItems = new List<OrderItems>();

            foreach (var orderItem in orderDto.OrderItemList)
            {
                orderItems.Add(
                new OrderItems
                {
                    OrderId = orderEntityEntry.OrderId,
                    EmployeeId = orderEntityEntry.EmployeeId,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Amount,
                });
            }

            return orderItems;
        }

        private Payments CreatePayment(Orders orderEntityEntry, OrderDto orderDto)
        {
            return new Payments
            {
                OrderId = orderEntityEntry.OrderId,
                Amount = orderDto.AmountToPay,
                PaymentMethod = orderDto.PaymentMethod
            };
        }
    }
}
