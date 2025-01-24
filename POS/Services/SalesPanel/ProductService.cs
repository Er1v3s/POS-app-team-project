using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace POS.Services.SalesPanel
{
    public class ProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly List<Product> productList;

        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            productList = Task.Run(GetAllProductsFromDb).Result;
        }

        public List<Product> LoadAllProducts()
        {
            return productList.ToList();
        }

        public List<Product> LoadProductsByCategory(object category)
        {
            return productList.Where(p => p.Category == category.ToString()).ToList();
        }

        public List<Product> LoadProductsBySearch(string searchText)
        {
            return productList.Where(p => p.ProductName.ToLower().Contains(searchText.ToLower())).ToList();
        }

        private async Task<List<Product>> GetAllProductsFromDb()
        {
            return await _dbContext.Product.ToListAsync();
        }
    }
}
