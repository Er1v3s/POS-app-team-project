using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DbSeeder
{
    public class Randomizer
    {
        private static readonly Random _random = new ();

        public static DateTime GenerateAlmostRandomDateTime()
        {
            DateTime startDate = new DateTime(2018, 1, 1);
            DateTime endDate = DateTime.Now.AddDays(1);

            int range = (endDate - startDate).Days;

            List<DayOfWeek> weightedDays = new List<DayOfWeek>
            {
                DayOfWeek.Sunday, DayOfWeek.Sunday,  // Sunday 2x
                DayOfWeek.Monday,
                DayOfWeek.Thursday, DayOfWeek.Thursday, // Thursday 2x
                DayOfWeek.Friday, DayOfWeek.Friday, DayOfWeek.Friday, DayOfWeek.Friday, DayOfWeek.Friday, // Friday 5x
                DayOfWeek.Saturday, DayOfWeek.Saturday, DayOfWeek.Saturday  // Saturday 3x
            };

            DayOfWeek randomDayOfWeek = weightedDays[_random.Next(weightedDays.Count)];

            DateTime randomDate;
            do
            {
                randomDate = startDate.AddDays(_random.Next(range + 1));
            } while (randomDate.DayOfWeek != randomDayOfWeek);

            int randomHour = _random.Next(20, 24);

            if (randomHour == 23)
            {
                randomHour = _random.Next(0, 5);
                randomDate = randomDate.AddDays(1);
            }

            int randomMinute = _random.Next(0, 60);
            int randomSecond = _random.Next(0, 60);

            randomDate = randomDate.AddHours(randomHour).AddMinutes(randomMinute).AddSeconds(randomSecond);

            return randomDate;
        }

        public static int GenerateAlmostRandomInt()
        {
            int temp = _random.Next(1, 21);
            int number;

            switch (temp)
            {
                case 20:
                    number = _random.Next(5, 11);
                    break;
                case 18 or 19:
                    number = _random.Next(3, 6);
                    break;
                case >= 15 and < 18:
                    number = _random.Next(2, 4);
                    break;
                default:
                    number = 1;
                    break;
            }

            return number;
        }

        public static string GenerateRandomPaymentMethod()
        {
            return _random.Next(1, 11) >= 7 ? "Gotówka" : "Karta debetowa";
        }

        public static async Task<Product> GenerateAlmostRandomProduct(AppDbContext dbContext, DayOfWeek dayOfWeek)
        {
            List<Product> productList = await dbContext.Product.ToListAsync();
            Product product;

            switch (dayOfWeek)
            {
                case DayOfWeek.Friday:
                {
                    product = GenerateProduct(productList, "Wódka");
                    break;
                }
                case DayOfWeek.Saturday:
                {
                    product = GenerateProduct(productList, "Rum");
                    break;
                }
                case DayOfWeek.Sunday:
                {
                    product = GenerateProduct(productList, "Whisky");
                    break;
                }
                default:
                {
                    var randomIndex = _random.Next(0, productList.Count);
                    product = productList[randomIndex];
                    break;
                }
            }

            return product;
        }

        private static Product GenerateProduct(List<Product> productList, string categoryName)
        {
            int threshold = categoryName switch
            {
                "Wódka" => 12,
                "Rum" => 7,
                "Whisky" => 6
            };

            if (_random.Next(1, threshold) >= 5)
            {
                var selectedByCategoryProductsList = productList
                    .Where(c => c.Category == categoryName)
                    .ToList();

                if (selectedByCategoryProductsList.Any())
                {
                    int randomIndex = _random.Next(0, selectedByCategoryProductsList.Count);
                    return selectedByCategoryProductsList[randomIndex];
                }
            }

            int randomProductIndex = _random.Next(0, productList.Count);
            return productList[randomProductIndex];
        }
    }
}
