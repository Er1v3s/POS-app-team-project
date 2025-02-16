using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Models.Orders;
using POS.Utilities;
using POS.Validators.Models;

namespace POS.Services
{
    public class IngredientService
    {
        private readonly AppDbContext _dbContext;
        private readonly DatabaseErrorHandler _databaseErrorHandler;
        private readonly IngredientValidator _ingredientValidator;

        private List<Ingredient> allIngredientList = new();
        public MyObservableCollection<Ingredient> IngredientCollection { get; }

        public IngredientService(AppDbContext dbContext, DatabaseErrorHandler databaseErrorHandler)
        {
            _dbContext = dbContext;
            _databaseErrorHandler = databaseErrorHandler;
            _ingredientValidator = new IngredientValidator();

            IngredientCollection = new();
            _ = GetAllIngredientsFromDbAsync();
        }

        public void GetAllIngredients()
        {
            ReloadCollection(IngredientCollection);
        }

        public void GetIngredientsBySearchPhrase(string searchText)
        {
            var filteredIngredients = allIngredientList.Where(i => i.Name.ToLower().Contains(searchText.ToLower()));
            IngredientCollection.Clear();
            IngredientCollection.AddRange(filteredIngredients);
        }

        public async Task AddNewIngredientAsync(Ingredient ingredient)
        {
            if (ingredient == null) throw new ArgumentNullException($"Niepoprawny składnik: {ingredient}");

            var validationResult = await _ingredientValidator.ValidateAsync(ingredient);
            if (!validationResult.IsValid) throw new ValidationException($"Składnik zawiera niepoprawne dane: \n{validationResult.ToString($"\n")}");

            allIngredientList.Add(ingredient);
            ReloadCollection(IngredientCollection);
            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(Ingredient ingredient)
        {
            if(ingredient == null) throw new ArgumentNullException($"Niepoprawny produkt: {ingredient}");

            allIngredientList.Remove(ingredient);
            ReloadCollection(IngredientCollection);

            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        private Ingredient GetIngredientFromCollection(Ingredient ingredient)
        {
            if (ingredient == null) throw new ArgumentNullException($"Niepoprawny produkt: {ingredient}");
            if (ingredient.Stock < 0) throw new ArgumentException($"Niepoprawna ilość składnika: {ingredient.Stock}");
            if (ingredient.SafetyStock < 0) throw new ArgumentException($"Niepoprawna ilość stanu bezpieczeństwa: {ingredient.SafetyStock}");

            var ingredientFromCollectionToUpdate = allIngredientList.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);

            if (ingredientFromCollectionToUpdate == null) throw new NotFoundException($"Nie odnaleziono składnika o Id: {ingredient.IngredientId}");

            return ingredientFromCollectionToUpdate;
        }

        public async Task UpdateIngredientQuantityAsync(Ingredient ingredient)
        {

            var ingredientFromCollectionToUpdate = GetIngredientFromCollection(ingredient);

            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                var ingredientToUpdate = await _dbContext.Ingredients
                    .Where(i => i.IngredientId == ingredient.IngredientId)
                    .FirstOrDefaultAsync();


                if (ingredientToUpdate == null || ingredientFromCollectionToUpdate == null)
                    throw new NotFoundException($"Nie odnaleziono składnika o Id: {ingredient.IngredientId}");

                ingredientToUpdate.Stock = ingredient.Stock;
                ingredientToUpdate.SafetyStock = ingredient.SafetyStock;
                await _dbContext.SaveChangesAsync();
            });

            ingredientFromCollectionToUpdate.Stock = ingredient.Stock;
            ingredientFromCollectionToUpdate.SafetyStock = ingredient.SafetyStock;
            ReloadCollection(IngredientCollection);
        }

        public async Task RemoveIngredientsAsync(OrderDto orderDto)
        {
            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
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
            });
        }

        public async Task<List<Ingredient>> GetRunningOutOfIngredientsAsync()
        {
            return await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                var runningOutOfIngredients = await _dbContext.Ingredients
                    .Where(ingredient => ingredient.Stock < ingredient.SafetyStock)
                    .ToListAsync();

                return runningOutOfIngredients;
            });
        }

        private async Task GetAllIngredientsFromDbAsync()
        {
            await _databaseErrorHandler.ExecuteDatabaseOperationAsync(async () =>
            {
                allIngredientList = await _dbContext.Ingredients.ToListAsync();
            });

            ReloadCollection(IngredientCollection);
        }

        private void ReloadCollection(MyObservableCollection<Ingredient> collection)
        {
            collection.Clear();
            collection.AddRange(allIngredientList);
        }
    }
}