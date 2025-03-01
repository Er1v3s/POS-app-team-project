using System.Linq;
using DataAccess.Models;
using POS.Validators.Models;
using System.Threading.Tasks;
using DataAccess;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Exceptions.Interfaces;
using POS.Utilities;
using System;

namespace POS.Services
{
    public class RecipeIngredientService
    {
        private readonly AppDbContext _dbContext;
        private readonly RecipeService _recipeService;
        private readonly IDatabaseErrorHandler _databaseErrorHandler;
        private readonly RecipeIngredientValidator _recipeIngredientValidator;
        public MyObservableCollection<RecipeIngredient> RecipeIngredientCollection { get; }

        public RecipeIngredientService(AppDbContext dbContext, IDatabaseErrorHandler databaseErrorHandler, RecipeService recipeService)
        {
            _dbContext = dbContext;
            _recipeService = recipeService;
            _databaseErrorHandler = databaseErrorHandler;
            _recipeIngredientValidator = new RecipeIngredientValidator();

            RecipeIngredientCollection = new();
        }

        public async Task AddIngredientToRecipeAsync(RecipeIngredient recipeIngredient)
        {
            if (recipeIngredient is null)
                throw new ArgumentNullException(nameof(recipeIngredient), "Niepoprawny składnik");

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeIngredient.RecipeId);

            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                recipe.RecipeIngredients.Add(recipeIngredient);
                await _dbContext.SaveChangesAsync();

                RecipeIngredientCollection.Add(recipeIngredient);
            });
        }

        public async Task UpdateIngredientInRecipeAsync(RecipeIngredient recipeIngredient, RecipeIngredient newRecipeIngredient)
        {
            if (recipeIngredient == null)
                throw new ArgumentNullException(nameof(recipeIngredient), "Nie wybrano składnika do edycji");
            if (newRecipeIngredient == null)
                throw new ArgumentNullException(nameof(newRecipeIngredient), "Niepoprawny nowy składnik");

            recipeIngredient.Quantity = newRecipeIngredient.Quantity;

            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                _dbContext.RecipeIngredients.Update(recipeIngredient);
                await _dbContext.SaveChangesAsync();

                var index = RecipeIngredientCollection.IndexOf(recipeIngredient);
                if (index != -1)
                    RecipeIngredientCollection[index] = recipeIngredient;
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

                RecipeIngredientCollection.Remove(recipeIngredient);
            });
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

        public async Task GetRecipeIngredientsAsync(Recipe recipe)
        {
            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                var recipeIngredients = await _dbContext.RecipeIngredients
                    .Include(ri => ri.Ingredient)
                    .Where(ri => ri.RecipeId == recipe.RecipeId)
                    .ToListAsync();

                if (recipeIngredients is null) throw new NotFoundException();

                RecipeIngredientCollection.Clear();
                RecipeIngredientCollection.AddRange(recipeIngredients);
            });
        }
    }
}
