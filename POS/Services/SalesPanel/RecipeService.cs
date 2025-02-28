using System;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POS.Exceptions;
using POS.Exceptions.Interfaces;
using POS.Validators.Models;

namespace POS.Services.SalesPanel
{
    public class RecipeService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDatabaseErrorHandler _databaseErrorHandler;

        private readonly RecipeValidator _recipeValidator;
        private readonly RecipeIngredientValidator _recipeIngredientValidator;

        public RecipeService(AppDbContext dbContext, IDatabaseErrorHandler databaseErrorHandler)
        {
            _dbContext = dbContext;
            _databaseErrorHandler = databaseErrorHandler;

            _recipeValidator = new RecipeValidator();
            _recipeIngredientValidator = new RecipeIngredientValidator();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int recipeId)
        {
            return await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                var recipe = await _dbContext.Recipe
                    .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefaultAsync(p => p.RecipeId == recipeId);

                if (recipe is null)
                    throw new NotFoundException("Przepis nie został odnaleziony");

                return recipe;
            });
        }

        public async Task AddIngredientToRecipeAsync(int recipeId, Ingredient ingredient, string amountOfIngredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient), "Niepoprawny składnik");
            if (amountOfIngredient.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(amountOfIngredient), "Niepoprawna ilość składnika");

            var recipe = await GetRecipeByIdAsync(recipeId);

            var recipeIngredient = await CreateRecipeIngredient(recipeId, ingredient, amountOfIngredient);

            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                recipe.RecipeIngredients.Add(recipeIngredient);
                await _dbContext.SaveChangesAsync();
            });
        }

        public async Task DeleteIngredientFromRecipeAsync(RecipeIngredient recipeIngredient)
        {
            if (recipeIngredient == null)
                throw new ArgumentNullException(nameof(recipeIngredient), "Nie wybrano składnika do usunięcia z przepisu");

            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                _dbContext.RecipeIngredients.Remove(recipeIngredient);
                await _dbContext.SaveChangesAsync();
            });
        }

        public async Task<Recipe> CreateRecipe(string productName, string recipeContent)
        {
            var newRecipe = new Recipe
            {
                RecipeName = $"Przepis na {productName}",
                RecipeContent = recipeContent
            };

            var validationResult = await _recipeValidator.ValidateAsync(newRecipe);
            if (!validationResult.IsValid) throw new ValidationException($"Produkt zawiera niepoprawne dane: \n{validationResult.ToString($"\n")}");

            return newRecipe;
        }

        private async Task<RecipeIngredient> CreateRecipeIngredient(int recipeId, Ingredient ingredient, string amountOfIngredient)
        {
            var newRecipeIngredient = new RecipeIngredient
            {
                RecipeId = recipeId,
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