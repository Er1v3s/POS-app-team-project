using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Models.Orders;
using POS.Utilities;

namespace POS.Services
{
    public class IngredientService
    {
        private readonly AppDbContext _dbContext;

        public MyObservableCollection<Ingredient> IngredientCollection { get; }

        public IngredientService(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            IngredientCollection = new();
            _ = GetAllIngredientsFromDbAsync();
        }

        public async Task GetAllIngredients()
        {
            IngredientCollection.Clear();
            await GetAllIngredientsFromDbAsync();
        }

        public void GetIngredientsBySearchPhrase(string searchText)
        {
            var filteredIngredients = IngredientCollection.Where(i => i.Name.ToLower().Contains(searchText.ToLower()));
            IngredientCollection.AddRange(filteredIngredients);
        }

        public async Task AddNewIngredientAsync(Ingredient ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException($"Niepoprawny składnik: {ingredient}");

            IngredientCollection.Add(ingredient);
            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(Ingredient ingredient)
        {
            if(ingredient == null)
                throw new ArgumentNullException($"Niepoprawny produkt: {ingredient}");

            IngredientCollection.Remove(ingredient);
            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateIngredientQuantityAsync(Ingredient ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException($"Niepoprawny produkt: {ingredient}");
            if (ingredient.Stock < 0)
                throw new ArgumentException($"Niepoprawna ilość składnika: {ingredient}");
            if (ingredient.SafetyStock < 0)
                throw new ArgumentException($"Niepoprawna ilość stanu bezpieczeństwa: {ingredient}");

            var ingredientToUpdate = await _dbContext.Ingredients
                .Where(i => i.IngredientId == ingredient.IngredientId)
                .FirstOrDefaultAsync();

            var ingredientFromCollectionToUpdate = IngredientCollection.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);

            if (ingredientToUpdate == null || ingredientFromCollectionToUpdate == null)
                throw new NotFoundException($"Nie odnaleziono składnika o Id: {ingredient.IngredientId}"); 

            ingredientFromCollectionToUpdate.Stock = ingredient.Stock;
            ingredientFromCollectionToUpdate.SafetyStock = ingredient.SafetyStock;

            ingredientToUpdate.Stock = ingredient.Stock;
            ingredientToUpdate.SafetyStock = ingredient.SafetyStock;
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveIngredientsAsync(OrderDto orderDto)
        {
            foreach (var orderItem in orderDto.OrderItemList)
            {
                var recipeId = await _dbContext.Product
                    .Where(p => p.ProductId == orderItem.ProductId)
                    .Select(r => r.RecipeId)
                    .FirstOrDefaultAsync();

                var recipeIngredientId = await _dbContext.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipeId)
                    .Select(ri => ri.IngredientId)
                    .ToListAsync();

                foreach (var ingredientId in recipeIngredientId)
                {
                    var ingredient = await _dbContext.Ingredients
                        .Where(i => i.IngredientId == ingredientId)
                        .FirstOrDefaultAsync();

                    var recipeIngredient = await _dbContext.RecipeIngredients
                        .Where(ri => ri.IngredientId == ingredientId)
                        .Select(ri => ri.Quantity)
                        .FirstOrDefaultAsync();

                    if (ingredient != null)
                        ingredient.Stock -= (int)recipeIngredient;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Ingredient>> GetRunningOutOfIngredientsAsync()
        {
            var runningOutOfIngredients = await _dbContext.Ingredients
                .Where(ingredient => ingredient.Stock < ingredient.SafetyStock)
                .ToListAsync();

            return runningOutOfIngredients;
        }

        private async Task GetAllIngredientsFromDbAsync()
        {
            var ingredients = await _dbContext.Ingredients.ToListAsync();

            if (ingredients.Count == 0)
                throw new NotFoundException("Nie odnaleziono żadnych składników");

            IngredientCollection.AddRange(ingredients);
        }
    }
}