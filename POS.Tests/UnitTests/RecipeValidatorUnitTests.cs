using DataAccess.Models;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class RecipeValidatorUnitTests
    {
        private readonly RecipeValidator _recipeValidator;

        public RecipeValidatorUnitTests()
        {
            _recipeValidator = new RecipeValidator();
        }

        private readonly Recipe recipe = new()
        {
            RecipeName = "test name",
            RecipeContent = "test content"
        };

        #region Validate Recipe Name

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return [""];
            yield return ["abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~"];
            yield return [new string('A', 201)];
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return ["ValidName"];
            yield return ["Valid Name 123"];
            yield return ["ValidName123"];
            yield return [new string('A', 200)];
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void RecipeName_ForInvalidValues_ShouldHaveValidationError(string invalidName)
        {
            // Arrange
            recipe.RecipeName = invalidName;

            // Act
            var result = _recipeValidator.TestValidate(recipe);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.RecipeName);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void RecipeName_ForValidValues_ShouldNotHaveValidationError(string validName)
        {
            // Arrange
            recipe.RecipeName = validName;

            // Act
            var result = _recipeValidator.TestValidate(recipe);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.RecipeName);
        }

        #endregion

        #region Validate Recipe Content

        public static IEnumerable<object[]> InvalidRecipeContent()
        {
            yield return [""];
            yield return ["abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~"];
            yield return [new string('A', 1001)];
        }

        public static IEnumerable<object[]> ValidRecipeContent()
        {
            yield return ["ValidContent"];
            yield return ["Valid Content 123"];
            yield return ["ValidContent123"];
            yield return [new string('A', 1000)];
        }

        [Theory]
        [MemberData(nameof(InvalidRecipeContent))]
        public void RecipeContent_ForInvalidValues_ShouldHaveValidationError(string content)
        {
            // Arrange
            recipe.RecipeContent = content;
            
            // Act
            var result = _recipeValidator.TestValidate(recipe);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.RecipeContent);
        }

        [Theory]
        [MemberData(nameof(ValidRecipeContent))]
        public void RecipeContent_ForValidValues_ShouldNotHaveValidationError(string content)
        {
            // Arrange
            recipe.RecipeContent = content;

            // Act
            var result = _recipeValidator.TestValidate(recipe);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.RecipeContent);
        }

        #endregion
    }
}
