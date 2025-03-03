using DataAccess;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Services.SalesPanel;

namespace POS.Tests.IntegrationTests
{
    public class ProductServiceIntegrationTests : IntegrationTestBase
    {
        private readonly ProductService _productService;

        public ProductServiceIntegrationTests()
        {
            _productService = new ProductService(_dbContext, _databaseErrorHandlerMock.Object);
        }

        [Fact]
        public void ProductService_OnServiceInitialize_GetDataFromDbToProductCollection()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            // Act 
            var productService = new ProductService(dbContext, _databaseErrorHandlerMock.Object);

            // Assert 
            productService.ProductCollection.Should().NotBeEmpty();
        }

        

        protected override async Task SeedDatabase(AppDbContext dbContext)
        {
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = "Tom Collins",
                    Category = "Whisky",
                    Price = 25.99,
                    Description = "Test description",
                    Recipe = new Recipe { RecipeName = "Recipe name", RecipeContent = "Recipe content" }
                },
                new Product
                {
                    ProductName = "Negroni",
                    Category = "Gin",
                    Price = 23.99,
                    Description = "Test description",
                    Recipe = new Recipe { RecipeName = "Recipe name", RecipeContent = "Recipe content" }
                },
                new Product
                {
                    ProductName = "Mad dog",
                    Category = "Wódka",
                    Price = 18.99,
                    Description = "Test description",
                    Recipe = new Recipe { RecipeName = "Recipe name", RecipeContent = "Recipe content" }
                }
            };
            await dbContext.Product.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
        }
    }
}
