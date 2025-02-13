using DataAccess.Models;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests
{
    public class IngredientValidatorTests
    {
        private readonly IngredientValidator _validator;

        public IngredientValidatorTests()
        {
            _validator = new IngredientValidator();
        }

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { "abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~" };
            yield return new object[] { new string('A', 101) };
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void Name_Should_Have_Error_For_Invalid_Values(string invalidName)
        {
            // Arrange
            var ingredient = new Ingredient()
            {
                Name = invalidName,
                Description = "Valid",
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return new object[] { "ValidName" };
            yield return new object[] { "Valid Name 123" };
            yield return new object[] { "ValidName123" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void Name_Should_Not_Have_Error_For_Valid_Values(string validName)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = validName,
                Description = "Valid",
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        public static IEnumerable<object[]> InvalidDescription()
        {
            yield return new object[] { "" };
            yield return new object[] { "`~@#$^&*-_=+[]{};:<>|" };
            yield return new object[] { new string('A', 401) };
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void Description_Should_Have_Error_For_Invalid_Values(string invalidDescription)
        {
            // Arrange
            var ingredient = new Ingredient()
            {
                Name = "Valid Name",
                Description = invalidDescription,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        public static IEnumerable<object[]> ValidDescription()
        {
            yield return new object[] { "ValidDescription 123 !.,()" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void Description_Should_Not_Have_Error_For_Valid_Values(string validDescription)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid Name",
                Description = validDescription,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Unit_Should_Have_Error_For_Invalid_Values()
        {
            // Arrange
            var longUnit = new string('A', 101);

            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Unit = longUnit,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Unit);
        }

        [Fact]
        public void Package_Should_Have_Error_For_Invalid_Values()
        {
            // Arrange
            var longPackage = new string('A', 101);

            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Package = longPackage,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Package);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void Stock_Should_Have_Error_For_Invalid_Values(int invalidStockValue)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Stock = invalidStockValue,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Stock);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void SafetyStock_Should_Have_Error_For_Invalid_Values(int invalidSafetyStockValue)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Stock = 5,
                SafetyStock = invalidSafetyStockValue,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Arrange
            result.ShouldHaveValidationErrorFor(x => x.SafetyStock);
        }
    }
}