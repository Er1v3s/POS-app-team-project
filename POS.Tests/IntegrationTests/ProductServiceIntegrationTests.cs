﻿using DataAccess;
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

        [Fact]
        public async Task GetAllProducts_ForCallingTheMethod_ReturnEntireCollectionOfProducts()
        {
            // Arrange
            var countOfAllItems = await _dbContext.Product.CountAsync();

            // Act
            var collectionOfAllProducts = _productService.GetAllProducts();

            // Assert 
            collectionOfAllProducts.Should().NotBeEmpty();
            collectionOfAllProducts.Should().HaveCount(countOfAllItems);
        }

        [Fact]
        public void GetProductsBySearchPhrase_ForPassedPhrase_ReturnsCorrectProductsCollection()
        {
            // Arrange
            const string searchPhrase = "Neg";

            // Act
            var collectionOfProductContainingSearchPhrase = _productService.GetProductsBySearchPhrase(searchPhrase);

            // Assert
            collectionOfProductContainingSearchPhrase.Should().NotBeEmpty();
            collectionOfProductContainingSearchPhrase.Should().OnlyContain(p => p.ProductName.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void GetProductsBySearchPhrase_ForNonExistentPhrase_ReturnsEmptyCollection()
        {
            // Arrange
            const string searchPhrase = "NonExistentProduct";

            // Act
            var collectionOfProducts = _productService.GetProductsBySearchPhrase(searchPhrase);

            // Assert
            collectionOfProducts.Should().BeEmpty();
        }


        [Fact]
        public void GetProductsByCategory_ForPassedCategory_ReturnsCorrectProductCollection()
        {
            // Arrange
            const string category = "Whisky";

            // Act
            var collectionOfProductsSelectedByCategory = _productService.GetProductsByCategory(category);

            // Assert
            collectionOfProductsSelectedByCategory.Should().NotBeEmpty();
            collectionOfProductsSelectedByCategory.Should().OnlyContain(p => p.Category.Equals(category));
        }

        [Fact]
        public void GetProductsByCategory_ForNonExistingCategory_ReturnsEmptyCollection()
        {
            // Arrange
            const string category = "NonExistentCategory";

            // Act
            var products = _productService.GetProductsByCategory(category);

            // Assert
            products.Should().BeEmpty();
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
