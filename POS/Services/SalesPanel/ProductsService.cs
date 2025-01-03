using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace POS.Services.SalesPanel
{
    public class ProductsService(AppDbContext dbContext)
    {
        public async Task<List<Products>> LoadAllProducts()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<List<Products>> LoadProductsByCategory(object category)
        {
            return await dbContext.Products.Where(p => p.Category == category.ToString()).ToListAsync();
        }

        public async Task<List<Products>> LoadProductsBySearch(string searchText)
        {
            return await dbContext.Products.Where(p => p.ProductName.ToLower().Contains(searchText.ToLower())).ToListAsync();
        }
    }
}
