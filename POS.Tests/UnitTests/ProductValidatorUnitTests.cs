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

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { "abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~" };
            yield return new object[] { new string('A', 101) };
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return new object[] { "ValidName" };
            yield return new object[] { "Valid Name 123" };
            yield return new object[] { "ValidName123" };
            yield return new object[] { new string('A', 100) };
        }

        #region Validate Product Name

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void ProductName_ForInvalidValues_ShouldHaveValidationError(string invalidName)
        {
            // Arrange
            product.ProductName = invalidName;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.ProductName);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void ProductName_ForValidValues_ShouldNotHaveValidationError(string validName)
        {
            // Arrange
            product.ProductName = validName;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldNotHaveValidationErrorFor(p => p.ProductName);
        }

        #endregion
    }
}
