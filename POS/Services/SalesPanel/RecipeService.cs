using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Models.Orders;

namespace POS.Services.SalesPanel
{
    public class RecipeService
    {
        private readonly AppDbContext _dbContext;

        public RecipeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Recipes> GetRecipe(OrderItemDto orderItem)
        {
            return (await _dbContext.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(p => p.RecipeId == orderItem.RecipeId))!;
        }
    }
}