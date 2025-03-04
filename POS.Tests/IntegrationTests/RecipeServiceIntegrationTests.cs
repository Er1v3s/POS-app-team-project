using DataAccess;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions;
using POS.Services;

namespace POS.Tests.IntegrationTests
{
    public class RecipeServiceIntegrationTests : IntegrationTestBase
    {
        private readonly RecipeService _recipeService;

        public RecipeServiceIntegrationTests() : base(nameof(RecipeServiceIntegrationTests))
        {
            _recipeService = new RecipeService(_dbContext, _databaseErrorHandlerMock.Object);
        }

        [Fact]
        public async Task GetRecipeByIdAsync_ForThePassedArgument_ReturnsRecipe()
        {
            // Arrange
            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task<Recipe>>>(), It.IsAny<Action>()))
                .Returns<Func<Task<Recipe>>, Action<Exception>>((operation, onFailure) => operation());

            var existingRecipe = await _dbContext.Recipe.FirstOrDefaultAsync();
            if (existingRecipe is null) throw new NotFoundException();

            // Act
            var recipeService = new RecipeService(_dbContext, _databaseErrorHandlerMock.Object);
            var recipe = await recipeService.GetRecipeByIdAsync(existingRecipe.RecipeId);

            // Assert
            recipe.Should().NotBeNull();
            recipe.RecipeId.Should().Be(existingRecipe.RecipeId);
            recipe.RecipeIngredients.Should().NotBeNullOrEmpty();
            recipe.RecipeIngredients.First().Ingredient.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateRecipe_ForThePassedArguments_ReturnsRecipe()
        {
            // Arrange
            const string productName = "Test product name";
            const string recipeContent = "Test recipe content";

            // Act
            var recipe = await _recipeService.CreateRecipe(productName, recipeContent);

            // Assert
            recipe.Should().NotBeNull();
            recipe.RecipeName.Should().Contain(productName);
            recipe.RecipeContent.Should().Be(recipeContent);
        }

        protected override async Task SeedDatabase(AppDbContext dbContext)
        {
            var ingredient = new Ingredient()
            {
                Name = "Whisky",
                Description = "Jack Daniel's",
                Unit = "szt",
                Package = "Szklana butelka 700ml",
                Stock = 10,
                SafetyStock = 5
            };

            var recipe = new Recipe()
            {
                RecipeName = "Test recipe name",
                RecipeContent = "Test recipe content",
            };

            var recipeIngredients = new List<RecipeIngredient>()
            {
                new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Quantity = 50,
                    Recipe = recipe
                },
            };

            recipe.RecipeIngredients = recipeIngredients;

            await dbContext.Ingredients.AddAsync(ingredient);
            await dbContext.Recipe.AddAsync(recipe);
            await dbContext.RecipeIngredients.AddRangeAsync(recipeIngredients);
            await dbContext.SaveChangesAsync();
        }
    }
}
