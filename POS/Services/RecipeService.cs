using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Exceptions.Interfaces;
using POS.Validators.Models;

namespace POS.Services
{
    public class RecipeService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDatabaseErrorHandler _databaseErrorHandler;

        private readonly RecipeValidator _recipeValidator;

        public RecipeService(AppDbContext dbContext, IDatabaseErrorHandler databaseErrorHandler)
        {
            _dbContext = dbContext;
            _databaseErrorHandler = databaseErrorHandler;

            _recipeValidator = new RecipeValidator();
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
    }
}