using DataAccess;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions;
using POS.Exceptions.Interfaces;
using POS.Models.Orders;
using POS.Services;

namespace POS.Tests.IntegrationTests
{
    public class IngredientServiceIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task IngredientService_OnServiceInitialize_GetDataFromDbToIngredientCollection()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            await SeedDatabase(dbContext);

            // Act 
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);

            // Assert 
            ingredientService.IngredientCollection.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllIngredients_WhenAnyFiltersAreAppliedToIngredientCollection_ReturnEntireCollectionWithoutFilters()
        {
            // Arrange
            var searchPhrase = "Whi";

            // Act
            var ingredientService = new IngredientService(_dbContext, _databaseErrorHandlerMock.Object);
            ingredientService.GetIngredientsBySearchPhrase(searchPhrase);
            // now IngredientCollection contains only one ingredient [ingredientService.IngredientCollection.Should().HaveCount(1)]

            ingredientService.GetAllIngredients();

            // Assert 
            ingredientService.IngredientCollection.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetIngredientsBySearchPhrase_ForPassedPhrase_AddCorrectIngredientsIntoCollection()
        {
            // Arrange
            var searchPhrase = "Whi";

            // Act
            var ingredientService = new IngredientService(_dbContext, _databaseErrorHandlerMock.Object);
            ingredientService.GetIngredientsBySearchPhrase(searchPhrase);

            // Assert
            ingredientService.IngredientCollection.Should().HaveCount(1);
            foreach (var ingredient in ingredientService.IngredientCollection)
            {
                ingredient.Name.Should().Be("Whisky");
            }
        }

        [Fact]
        public async Task AddNewIngredientAsync_OnAddNewIngredientAsync_AddNewIngredientToDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ingredientToAdd = new Ingredient() { IngredientId = 999,  Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.AddNewIngredientAsync(ingredientToAdd);

            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredientToAdd.IngredientId);

            // Assert
            ingredientFromDb.Should().NotBeNull();
            ingredientFromDb.IngredientId.Should().Be(ingredientToAdd.IngredientId);
        }

        [Fact]
        public async Task UpdateExistingIngredientAsync_OnUpdateExistingIngredientAsync_UpdateExistingIngredientInDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ingredientToUpdate = await dbContext.Ingredients.FirstOrDefaultAsync();
            if (ingredientToUpdate is null) throw new NotFoundException();

            var updatedIngredient = new Ingredient() { Name = "Updated name", Description = "Updated description", Unit = "Updated unit", Package = "Updated package"};

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.UpdateExistingIngredientAsync(ingredientToUpdate, updatedIngredient);

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredientToUpdate.IngredientId);

            ingredientFromDb.Should().NotBeNull();
            ingredientFromDb.IngredientId.Should().Be(ingredientToUpdate.IngredientId);
            ingredientFromDb.Name.Should().Be(updatedIngredient.Name);
            ingredientFromDb.Unit.Should().Be(updatedIngredient.Unit);
            ingredientFromDb.Package.Should().Be(updatedIngredient.Package);
            ingredientFromDb.Description.Should().Be(updatedIngredient.Description);
        }

        [Fact]
        public async Task DeleteIngredientAsync_OnDeleteIngredientAsync_DeleteIngredientFromDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ingredientToDelete = await dbContext.Ingredients.FirstOrDefaultAsync();
            if (ingredientToDelete is null) throw new NotFoundException();

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.DeleteIngredientAsync(ingredientToDelete);

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredientToDelete.IngredientId);
            ingredientFromDb.Should().BeNull();
        }

        [Fact]
        public async Task GetRunningOutOfIngredients_WhenStockIsLowerThanSafetyStock_ReturnsCorrectIngredients()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task<List<Ingredient>>>>(), It.IsAny<Action>()))
                .Returns<Func<Task<List<Ingredient>>>, Action<Exception>>((operation, onFailure) => operation());

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            var runningOutOfIngredients = await ingredientService.GetRunningOutOfIngredientsAsync();

            // Assert
            foreach (var runningOutOfIngredient in runningOutOfIngredients)
                runningOutOfIngredient.Stock.Should().BeLessThan(runningOutOfIngredient.SafetyStock);
        }

        [Fact]
        public async Task RemoveIngredientsAsync_ForPassedArgument_RemovesIngredientsFromDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            var recipe = new Recipe { RecipeName = "test name", RecipeContent = "test content" };
            var ingredient = await dbContext.Ingredients.FirstOrDefaultAsync();
            var ingredient2 = await dbContext.Ingredients.Skip(1).FirstOrDefaultAsync();

            if (ingredient is null || ingredient2 is null) throw new NotFoundException();

            await dbContext.Recipe.AddAsync(recipe);
            await dbContext.RecipeIngredients.AddRangeAsync(
                new RecipeIngredient { RecipeId = recipe.RecipeId, Recipe = recipe, IngredientId = ingredient.IngredientId, Ingredient = ingredient, Quantity = 1 },
                new RecipeIngredient { RecipeId = recipe.RecipeId, Recipe = recipe, IngredientId = ingredient2.IngredientId, Ingredient = ingredient2, Quantity = 1 });
            var product = new Product { RecipeId = recipe.RecipeId, ProductName = "test name", Category = "test category", Description = "test description", Price = 0, Recipe = recipe};
            await dbContext.Product.AddAsync(product);

            await dbContext.SaveChangesAsync();

            var orderDto = new OrderDto
            {
                EmployeeId = 0,
                OrderItemList = new List<OrderItemDto>
            {
                new() { ProductId = product.ProductId }
            },
            };

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.RemoveIngredientsAsync(orderDto);

            // Assert
            var updatedIngredient1 = await dbContext.Ingredients.FindAsync(ingredient.IngredientId);
            var updatedIngredient2 = await dbContext.Ingredients.FindAsync(ingredient2.IngredientId);

            updatedIngredient1?.Stock.Should().Be(9);
            updatedIngredient2?.Stock.Should().Be(6);
        }

        protected override async Task SeedDatabase(AppDbContext dbContext)
        {
            var ingredients = new List<Ingredient>
            {
                new Ingredient() { Name = "Whisky", Description = "Jack Daniel's", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 5 },
                new Ingredient() { Name = "Wódka", Description = "Finlandia", Unit = "szt", Package = "Szklana butelka 1000ml", Stock = 7, SafetyStock = 3 },
                new Ingredient() { Name = "Rum", Description = "Bacardi White", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 10 }
            };

            await dbContext.Ingredients.AddRangeAsync(ingredients);
            await dbContext.SaveChangesAsync();
        }
    }
}