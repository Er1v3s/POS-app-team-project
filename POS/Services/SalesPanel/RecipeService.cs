using System;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POS.Exceptions;

namespace POS.Services.SalesPanel
{
    public class RecipeService
    {
        private readonly AppDbContext _dbContext;

        public RecipeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Recipe> GetRecipeByIdAsync(int recipeId)
        {
            var recipe = await _dbContext.Recipe
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(p => p.RecipeId == recipeId);

            if(recipe == null)
                throw new NotFoundException("Przepis nie został odnaleziony");

            return recipe;
        }

        public async Task AddIngredientToRecipeAsync(int recipeId, Ingredient ingredient, string amountOfIngredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException($"Niepoprawny składnik {ingredient?.Name}");

            if (amountOfIngredient.IsNullOrEmpty())
                throw new ArgumentNullException($"Niepoprawna ilość składnika {amountOfIngredient}");

            var recipe = await GetRecipeByIdAsync(recipeId);

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredient.IngredientId,
                Ingredient = ingredient,
                Quantity = double.Parse(amountOfIngredient)
            });

           await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteIngredientFromRecipeAsync(RecipeIngredient recipeIngredient)
        {
            if (recipeIngredient == null)
                throw new ArgumentNullException($"Nie wybrano składnika do usunięcia z przepisu");

            _dbContext.RecipeIngredients.Remove(recipeIngredient);
            await _dbContext.SaveChangesAsync();
        }
    }
}