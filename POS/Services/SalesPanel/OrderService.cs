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

        public async Task HandleOrderAsync(OrderDto orderDto)
        {
            await SaveHandledOrderInDb(orderDto);
        }

        public void LoadFinishedOrdersWindow()
        {
            var finishedOrders = new FinishedOrdersWindow();
            finishedOrders.Show();
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

        private Order CreateOrder(OrderDto orderDto)
        {
            return new Order
            {
                EmployeeId = orderDto.EmployeeId,
                OrderTime = DateTime.Now,
                DayOfWeek = DateTime.Now.DayOfWeek
            };
        }

        private List<OrderItem> CreateOrderItemList(Order orderEntityEntry, OrderDto orderDto)
        {
            var orderItems = new List<OrderItem>();

            foreach (var orderItem in orderDto.OrderItemList)
            {
                orderItems.Add(
                new OrderItem
                {
                    OrderId = orderEntityEntry.OrderId,
                    EmployeeId = orderEntityEntry.EmployeeId,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Amount,
                });
            }

            return orderItems;
        }

        private Payment CreatePayment(Order orderEntityEntry, OrderDto orderDto)
        {
            return new Payment
            {
                OrderId = orderEntityEntry.OrderId,
                Amount = orderDto.AmountToPay,
                PaymentMethod = orderDto.PaymentMethod
            };
        }
    }
}
