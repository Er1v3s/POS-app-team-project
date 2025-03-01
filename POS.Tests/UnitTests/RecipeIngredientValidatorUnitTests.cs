using System.Globalization;
using DataAccess.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class RecipeIngredientValidatorUnitTests
    {
        private readonly RecipeIngredientValidator _recipeIngredientValidator;

        public RecipeIngredientValidatorUnitTests()
        {
            _recipeIngredientValidator = new RecipeIngredientValidator();
        }

        private RecipeIngredient recipeIngredient = new ()
        {
            Ingredient = new Ingredient() { Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package"},
            Recipe = new Recipe { RecipeName = "Test name", RecipeContent = "Test content" },
            Quantity = 0
        };

        #region Validate Quantity

        public static IEnumerable<object[]> InvalidQuantity()
        {
            yield return ["-1"];
            yield return ["50.501"];
            yield return ["250,501"];
            yield return ["1001"];
        }

        public static IEnumerable<object[]> ValidQuantity()
        {
            yield return ["0"];
            yield return ["50.50"];
            yield return ["300,50"];
            yield return ["1000"];
        }

        [Theory]
        [MemberData(nameof(InvalidQuantity))]
        public void RecipeIngredientQuantity_ForInvalidValues_ShouldHaveValidationError(string quantity)
        {
            // Arrange
            var quantityAsDouble = double.Parse(quantity, NumberStyles.Any, CultureInfo.InvariantCulture);
            recipeIngredient.Quantity = quantityAsDouble;

            // Act
            var result = _recipeIngredientValidator.TestValidate(recipeIngredient);

            // Assert
            result.ShouldHaveValidationErrorFor(ri => ri.Quantity);
        }

        [Theory]
        [MemberData(nameof(ValidQuantity))]
        public void RecipeIngredientQuantity_ForValidValues_ShouldNotHaveValidationError(string quantity)
        {
            // Arrange
            var quantityAsDouble = double.Parse(quantity.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
            recipeIngredient.Quantity = quantityAsDouble;

            // Act
            var result = _recipeIngredientValidator.TestValidate(recipeIngredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(ri => ri.Quantity);
        }

        #endregion
    }
}
