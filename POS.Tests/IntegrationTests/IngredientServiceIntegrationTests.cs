using DataAccess;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions.Interfaces;
using POS.Models.Orders;
using POS.Services;

namespace POS.Tests.IntegrationTests
{
    public class IngredientServiceIntegrationTests
    {
        private readonly Mock<IDatabaseErrorHandler> _databaseErrorHandlerMock;

        public IngredientServiceIntegrationTests()
        {
            _databaseErrorHandlerMock = new Mock<IDatabaseErrorHandler>();

            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()))
                .Returns<Func<Task>, Action<Exception>>((operation, onFailure) => operation());
        }


        [Fact]
        public async Task IngredientService_OnServiceInitialize_GetDataFromDbToIngredientCollection()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            await SeedDatabaseWithIngredients(dbContext);

            // Act 
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);

            // Assert 
            ingredientService.IngredientCollection.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllIngredients_WhenAnyFiltersAreAppliedToIngredientCollection_ReturnEntireCollectionWithoutFilters()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            await SeedDatabaseWithIngredients(dbContext);
            var searchPhrase = "Whi";

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
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
            var dbContext = GetInMemoryDbContext();
            await SeedDatabaseWithIngredients(dbContext);

            var searchPhrase = "Whi";

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
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
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.AddNewIngredientAsync(ingredient);

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FirstOrDefaultAsync();
            ingredientFromDb.Should().NotBeNull();
            ingredientFromDb.Name.Should().Be(ingredient.Name);
        }

        [Fact]
        public async Task UpdateExistingIngredientAsync_OnUpdateExistingIngredientAsync_UpdateExistingIngredientInDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await dbContext.Ingredients.AddAsync(ingredient);
            await dbContext.SaveChangesAsync();

            var updatedIngredient = new Ingredient() { IngredientId = 1, Name = "Updated name", Description = "Updated description", Unit = "Updated unit", Package = "Updated package"};

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.UpdateExistingIngredientAsync(ingredient, updatedIngredient);
            //await dbContext.SaveChangesAsync();

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredient.IngredientId);

            ingredientFromDb.Should().NotBeNull();
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
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await dbContext.Ingredients.AddAsync(ingredient);
            await dbContext.SaveChangesAsync();

            // Act
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            await ingredientService.DeleteIngredientAsync(ingredient);

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredient.IngredientId);
            ingredientFromDb.Should().BeNull();
        }

        [Fact]
        public async Task GetRunningOutOfIngredients_WhenStockIsLowerThanSafetyStock_ReturnsCorrectIngredients()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            await SeedDatabaseWithIngredients(dbContext);

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
            var ingredient = new Ingredient { Name = "Whisky", Description = "Jack Daniel's", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 5 };
            var ingredient2 = new Ingredient { Name = "Wódka", Description = "Finlandia", Unit = "szt", Package = "Szklana butelka 1000ml", Stock = 7, SafetyStock = 3 };

            await dbContext.Ingredients.AddRangeAsync(ingredient, ingredient2);
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

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            var dbContext = new AppDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        private async Task SeedDatabaseWithIngredients(AppDbContext dbContext)
        {
            await dbContext.Ingredients.AddAsync(new Ingredient() { Name = "Whisky", Description = "Jack Daniel's", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 5 });
            await dbContext.Ingredients.AddAsync(new Ingredient() { Name = "Wódka", Description = "Finlandia", Unit = "szt", Package = "Szklana butelka 1000ml", Stock = 7, SafetyStock = 3 });
            await dbContext.Ingredients.AddAsync(new Ingredient() { Name = "Rum", Description = "Bacardi White", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 10 });
            await dbContext.SaveChangesAsync();
        }
    }
}