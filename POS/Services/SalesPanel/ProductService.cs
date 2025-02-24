using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Utilities;
using POS.Validators.Models;

namespace POS.Services.SalesPanel
{
    public class ProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly ProductValidator _productValidator;
        public MyObservableCollection<Product> ProductCollection { get; }

        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _productValidator = new ProductValidator();

            ProductCollection = new();
            _ = GetAllProductsFromDbAsync();
        }

        public MyObservableCollection<Product> GetAllProducts()
        {
            return ProductCollection;
        }

        public ObservableCollection<Product> GetProductsByCategory(object category)
        {
            return new ObservableCollection<Product>(ProductCollection.Where(p => p.Category == category.ToString()));
        }

        public ObservableCollection<Product> GetProductsBySearchPhrase(string searchText)
        {
            return new ObservableCollection<Product>(ProductCollection.Where(p => p.ProductName.ToLower().Contains(searchText.ToLower())));
        }

        public async Task AddNewProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException($"Niepoprawny produt: {product}");

            ProductCollection.Add(product);
            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException($"Niepoprawny produkt: {product}");

            ProductCollection.Remove(product);
            _dbContext.Product.Remove(product);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<Product> CreateProduct(string productName, string productCategory, string productDescription, string productPrice)
        {
            var newProduct = new Product
            {
                ProductName = productName,
                Category = productCategory,
                Description = productDescription,
                Price = double.Parse(productPrice),
            };

            var validationResult = await _productValidator.ValidateAsync(newProduct);
            if (!validationResult.IsValid) throw new ValidationException($"Produkt zawiera niepoprawne dane: \n{validationResult.ToString($"\n")}");

            return newProduct;
        }

        private async Task GetAllProductsFromDbAsync()
        {
            var products = await _dbContext.Product.ToListAsync();

            if (products.Count == 0)
                throw new NotFoundException("Nie znaleziono żadnych produktów");

            ProductCollection.AddRange(products);
        }
    }
}