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
        private readonly AppDbContext _dbContext;
        private readonly IngredientService _ingredientService;

        public IngredientServiceIntegrationTests()
        {
            _databaseErrorHandlerMock = new Mock<IDatabaseErrorHandler>();

            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()))
                .Returns<Func<Task>, Action<Exception>>((operation, onFailure) => operation());

            _dbContext = GetInMemoryDbContext();

            _ingredientService = new IngredientService(_dbContext, _databaseErrorHandlerMock.Object);
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

        [Fact]
        public async Task IngredientService_OnServiceInitialize_GetDataFromDbToIngredientCollection()
        {
            // Arrange
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 });
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 2, Name = "Test name 2", Description = "Test description 2", Unit = "Test unit 2", Package = "Test package 2", Stock = 7, SafetyStock = 3 });
            await _dbContext.SaveChangesAsync();

            // Act 
            var ingredientService = new IngredientService(_dbContext, _databaseErrorHandlerMock.Object);

            // Assert 
            ingredientService.IngredientCollection.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetIngredientsBySearchPhrase_ForPassedPhrase_AddCorrectIngredientsIntoCollection()
        {
            // Arrange
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 1, Name = "Whisky", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 });
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 2, Name = "Wódka", Description = "Test description 2", Unit = "Test unit 2", Package = "Test package 2", Stock = 7, SafetyStock = 3 });
            await _dbContext.SaveChangesAsync();

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
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };

            // Act
            await _ingredientService.AddNewIngredientAsync(ingredient);

            // Assert
            var ingredientFromDb = await _dbContext.Ingredients.FirstOrDefaultAsync();
            ingredientFromDb.Should().NotBeNull();
            ingredientFromDb.Name.Should().Be(ingredient.Name);
        }

        [Fact]
        public async Task UpdateExistingIngredientAsync_OnUpdateExistingIngredientAsync_UpdateExistingIngredientInDb()
        {
            // Arrange
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();

            var updatedIngredient = new Ingredient() { IngredientId = 1, Name = "Updated name", Description = "Updated description", Unit = "Updated unit", Package = "Updated package"};

            // Act
            await _ingredientService.UpdateExistingIngredientAsync(ingredient, updatedIngredient);
            await _dbContext.SaveChangesAsync();

            // Assert
            var ingredientFromDb = await _dbContext.Ingredients.FindAsync(ingredient.IngredientId);

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
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();

            // Act
            await _ingredientService.DeleteIngredientAsync(ingredient);

            // Assert
            var ingredientFromDb = await _dbContext.Ingredients.FindAsync(ingredient.IngredientId);
            ingredientFromDb.Should().BeNull();
        }

        [Fact]
        public async Task GetRunningOutOfIngredients_WhenStockIsLowerThanSafetyStock_ReturnsCorrectIngredients()
        {
            // Arrange
            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task<List<Ingredient>>>>(), It.IsAny<Action>()))
                .Returns<Func<Task<List<Ingredient>>>, Action<Exception>>((operation, onFailure) => operation());

            // Must be initialized because of databaseErrorHandler with generic parameter
            var ingredientService = new IngredientService(_dbContext, _databaseErrorHandlerMock.Object);
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 1, Name = "Test name 1", Description = "Test description 1", Unit = "Test unit 1", Package = "Test package 1", Stock = 10, SafetyStock = 5 });
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 2, Name = "Test name 2", Description = "Test description 2", Unit = "Test unit 2", Package = "Test package 2", Stock = 10, SafetyStock = 10 });
            await _dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 3, Name = "Test name 3", Description = "Test description 3", Unit = "Test unit 3", Package = "Test package 3", Stock = 3, SafetyStock = 5 });
            await _dbContext.SaveChangesAsync();

            // Act
            var runningOutOfIngredients = await ingredientService.GetRunningOutOfIngredientsAsync();

            // Assert
            foreach (var runningOutOfIngredient in runningOutOfIngredients)
                runningOutOfIngredient.Stock.Should().BeLessThan(runningOutOfIngredient.SafetyStock);
        }

        [Fact]
        public async Task RemoveIngredientsAsync_ForPassedArgument_RemovesIngredientsFromDb()
        {
            // Arrange
            await _dbContext.Ingredients.AddAsync(new Ingredient { IngredientId = 1, Name = "Whisky", Description = "test description", Stock = 10 });
            await _dbContext.Ingredients.AddAsync(new Ingredient { IngredientId = 2, Name = "Wódka", Description = "test description" ,Stock = 5 });

            await _dbContext.Recipes.AddAsync(new Recipe { RecipeId = 1, RecipeName = "test name", RecipeContent = "test content" });
            
            // most important 
            await _dbContext.RecipeIngredients.AddAsync(new RecipeIngredient { RecipeId = 1, IngredientId = 1, Quantity = 1 });
            await _dbContext.RecipeIngredients.AddAsync(new RecipeIngredient { RecipeId = 1, IngredientId = 2, Quantity = 1 });
            // most important

            await _dbContext.Product.AddAsync(new Product { ProductId = 1, RecipeId = 1, ProductName = "test name", Category = "test category", Price = 0 });
            
            await _dbContext.SaveChangesAsync();

            var orderDto = new OrderDto
            {
                EmployeeId = 0,
                OrderItemList = new List<OrderItemDto>
                {
                    new() { ProductId = 1 }
                },
            };

            // Act
            await _ingredientService.RemoveIngredientsAsync(orderDto);

            // Assert
            var updatedIngredient1 = await _dbContext.Ingredients.FindAsync(1);
            var updatedIngredient2 = await _dbContext.Ingredients.FindAsync(2);

            updatedIngredient1?.Stock.Should().Be(9); // 10 existing - 1 from order
            updatedIngredient2?.Stock.Should().Be(4); // 5 existing - 1 from order
        }
    }
}