﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Exceptions;
using POS.Models.Orders;

namespace POS.Services
{
    public class IngredientService
    {
        private readonly AppDbContext _dbContext;

        public IngredientService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            var ingredients = await _dbContext.Ingredients.ToListAsync();

            if (ingredients == null)
                throw new NotFoundException("Nie odnaleziono żadnych składników");

            return ingredients;
        }

        public async Task AddNewIngredientAsync(Ingredient ingredient)
        {
            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(Ingredient ingredient)
        {
            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveIngredients(OrderDto orderDto)
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
    }
}
