using DataAccess.Models;
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


    }
}
