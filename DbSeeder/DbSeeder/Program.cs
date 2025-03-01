using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DbSeeder
{
    class Program
    {
        private static Random random = new();
        private static AppDbContext _dbContext;

        static async Task Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(new DatabaseConfiguration().GetConnectionString(), builder => builder.MigrationsAssembly("DbSeeder"))
                .Options;

            _dbContext = new AppDbContext(options);

            for (int i = 0; i <= 10; i++)
            {
                await SeedDatabase();

                Console.WriteLine($"Dodano element nr: {i}");
            }
        }

        private static async Task SeedDatabase()
        {
            var order = await GenerateOrder();
            await GenerateOrderItems(_dbContext, order);
            await GeneratePayment(order);
        }

        private static async Task<Order> GenerateOrder()
        {
            int employeeId = random.Next(1, 4);
            DateTime randomDate = Randomizer.GenerateAlmostRandomDateTime();

            Order order = new Order
            {
                EmployeeId = employeeId,
                OrderTime = randomDate,
                DayOfWeek = randomDate.DayOfWeek,
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return order;
        }

        private static async Task GenerateOrderItems(AppDbContext dbContext, Order order)
        {
            int productSum = Randomizer.GenerateAlmostRandomInt();

            while (productSum > 0)
            {
                int quantity = Randomizer.GenerateAlmostRandomInt();
                Product product = await Randomizer.GenerateAlmostRandomProduct(dbContext, order.DayOfWeek);

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

        private static async Task GeneratePayment(Order order)
        {
            double amount = 0;
            string paymentMethod = Randomizer.GenerateRandomPaymentMethod();

            var orderItems = await _dbContext.OrderItems.Where(a => a.OrderId == order.OrderId).ToListAsync();

            foreach (var orderItem in orderItems)
            {
                var price = await _dbContext.Product
                    .Where(a => a.ProductId == orderItem.ProductId)
                    .Select(a => a.Price)
                    .FirstOrDefaultAsync();

                if (price != null)
                {
                    amount += price * orderItem.Quantity;
                }
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                Amount = Math.Round(amount, 2)
            };

            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
