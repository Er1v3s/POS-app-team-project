using DataAccess;
using DataAccess.Models;

namespace DbSeeder
{
    class Program
    {
        static Random random = new();

        static async Task Main(string[] args)
        {
            await using AppDbContext dbContext = new AppDbContext();

            for (int i = 0; i <= 5000; i++)
            {
                await SeedDatabase(dbContext);

                Console.WriteLine($"Dodano element nr: {i}");
            }
        }

        private static async Task SeedDatabase(AppDbContext dbContext)
        {
            var order = await GenerateOrder(dbContext);
            await GenerateOrderItems(dbContext, order);
            await GeneratePayment(dbContext, order);
        }

        private static async Task<Order> GenerateOrder(AppDbContext dbContext)
        {
            int employeeId = random.Next(1, 4);
            DateTime randomDate = Randomizer.GenerateAlmostRandomDateTime();

            Order order = new Order
            {
                EmployeeId = employeeId,
                OrderTime = randomDate,
                DayOfWeek = randomDate.DayOfWeek,
            };

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return order;
        }

        private static async Task GenerateOrderItems(AppDbContext dbContext, Order order)
        {
            int productSum = Randomizer.GenerateAlmostRandomInt();

            while (productSum > 0)
            {
                int quantity = Randomizer.GenerateAlmostRandomInt();
                Product product = Randomizer.GenerateAlmostRandomProduct(dbContext, order.DayOfWeek);

                OrderItem orderItem = new OrderItem 
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    EmployeeId = order.EmployeeId,
                };
                
                await dbContext.OrderItems.AddAsync(orderItem);
                productSum--;
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task GeneratePayment(AppDbContext dbContext, Order order)
        {
            double amount = 0;
            string paymentMethod = Randomizer.GenerateRandomPaymentMethod();

            var orderItems = dbContext.OrderItems.Where(a => a.OrderId == order.OrderId).ToList();

            foreach (var orderItem in orderItems)
            {
                var price = dbContext.Product
                    .Where(a => a.ProductId == orderItem.ProductId)
                    .Select(a => a.Price)
                    .FirstOrDefault();

                if (price != null)
                {
                    amount += price.Value * orderItem.Quantity;
                }
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                Amount = Math.Round(amount, 2)
            };

            await dbContext.Payments.AddAsync(payment);
            await dbContext.SaveChangesAsync();
        }
    }
}
