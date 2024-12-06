using DataAccess.Models;

namespace DbSeeder
{
    class Program
    {
        static Random _random = new Random();

        static void Main(string[] args)
        {
            using AppDbContext dbContext = new AppDbContext();

            for (int i = 0; i <= 100000; i++)
            {
                SeedDatabase(dbContext);
            }
        }

        private static void SeedDatabase(AppDbContext dbContext)
        {
            var order = GenerateOrder(dbContext);
            GenerateOrderItems(dbContext, order);
            GeneratePayment(dbContext, order);
        }

        private static Orders GenerateOrder(AppDbContext dbContext)
        {
            int employeeId = _random.Next(1, 4);
            DateTime randomDate = Randomizer.GenerateAlmostRandomDateTime();

            Orders order = new Orders
            {
                EmployeeId = employeeId,
                OrderTime = randomDate,
                DayOfWeek = randomDate.DayOfWeek,
            };

            dbContext.Orders.AddAsync(order);
            dbContext.SaveChangesAsync();

            return order;
        }

        private static void GenerateOrderItems(AppDbContext dbContext, Orders order)
        {
            int productSum = Randomizer.GenerateAlmostRandomInt();

            while (productSum > 0)
            {
                int quantity = Randomizer.GenerateAlmostRandomInt();
                Products product = Randomizer.GenerateAlmostRandomProduct(dbContext, order.DayOfWeek);

                OrderItems orderItem = new OrderItems 
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    EmployeeId = order.EmployeeId,
                };
                
                dbContext.OrderItems.AddAsync(orderItem);
                productSum--;
            }

            dbContext.SaveChanges();
        }

        private static void GeneratePayment(AppDbContext dbContext, Orders order)
        {
            double amount = 0;
            string paymentMethod = Randomizer.GenerateRandomPaymentMethod();

            var orderItems = dbContext.OrderItems.Where(a => a.OrderId == order.OrderId).ToList();

            foreach (var orderItem in orderItems)
            {
                var price = dbContext.Products
                    .Where(a => a.ProductId == orderItem.ProductId)
                    .Select(a => a.Price)
                    .FirstOrDefault();

                if (price != null)
                {
                    amount += price.Value * orderItem.Quantity;
                }
            }

            var payment = new Payments
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                Amount = Math.Round(amount, 2)
            };

            dbContext.Payments.Add(payment);
            dbContext.SaveChanges();
        }
    }
}
