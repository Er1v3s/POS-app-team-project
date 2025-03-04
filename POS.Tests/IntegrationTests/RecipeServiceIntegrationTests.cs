using DataAccess;
using DataAccess.Models;
using POS.Services;

namespace POS.Tests.IntegrationTests
{
    public class RecipeServiceIntegrationTests : IntegrationTestBase
    {
        private readonly RecipeService _recipeService;

        public RecipeServiceIntegrationTests()
        {
            _recipeService = new RecipeService(_dbContext, _databaseErrorHandlerMock.Object);
        }



        protected override async Task SeedDatabase(AppDbContext dbContext)
        {
            List<Recipe> listOfRecipes = new()
            {
                new Recipe() { RecipeName = "Test recipe name", RecipeContent = "Test recipe content" },
                new Recipe() { RecipeName = "Test recipe name 2", RecipeContent = "Test recipe content 2" },
                new Recipe() { RecipeName = "Test recipe name 3", RecipeContent = "Test recipe content 3" },
            };

            await _dbContext.AddRangeAsync(listOfRecipes);
            await _dbContext.SaveChangesAsync();
        }
    }
}
