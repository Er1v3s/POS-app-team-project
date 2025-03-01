using DataAccess.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class ProductValidatorUnitTests
    {
        private readonly ProductValidator _productValidator;

        public ProductValidatorUnitTests()
        {
            _productValidator = new ProductValidator();
        }

        private readonly Product product = new()
        {
            ProductName = "test name",
            Category = "test category",
            Description = "test description",
            Price = 25.99,
            Recipe = new Recipe() { RecipeName = "test name", RecipeContent = "test content" },
        };

        
    }
}
