using DataAccess.Models;
using POS.Validators.Models;
using System.Threading.Tasks;
using FluentValidation;

namespace POS.Services
{
    public class RecipeIngredientService
    {
        private readonly RecipeIngredientValidator _recipeIngredientValidator;

        public RecipeIngredientService()
        {
            _recipeIngredientValidator = new RecipeIngredientValidator();
        }

        public async Task<RecipeIngredient> CreateRecipeIngredient(Recipe recipe, Ingredient ingredient, string amountOfIngredient)
        {
            var newRecipeIngredient = new RecipeIngredient
            {
                RecipeId = recipe.RecipeId,
                IngredientId = ingredient.IngredientId,
                Ingredient = ingredient,
                Quantity = double.Parse(amountOfIngredient)
            };

            var validationResult = await _recipeIngredientValidator.ValidateAsync(newRecipeIngredient);
            if (!validationResult.IsValid) throw new ValidationException($"Produkt zawiera niepoprawne dane: \n{validationResult.ToString($"\n")}");

            return newRecipeIngredient;
        }
    }
}
