using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using POS.Models.Orders;

namespace POS.Services.SalesPanel
{
    public class IngredientService
    {
        private readonly AppDbContext _dbContext;

        public IngredientService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
