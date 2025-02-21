using DataAccess;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions.Interfaces;
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
            var dbContext = GetInMemoryDbContext();

            await dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 });
            await dbContext.Ingredients.AddAsync(new Ingredient() { IngredientId = 2, Name = "Test name 2", Description = "Test description 2", Unit = "Test unit 2", Package = "Test package 2", Stock = 7, SafetyStock = 3 });
            await dbContext.SaveChangesAsync();

            // Act 
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);

            // Assert 
            ingredientService.IngredientCollection.Should().NotBeEmpty();
        }

        [Fact]
        public async Task AddNewIngredientAsync_OnAddNewIngredientAsync_AddNewIngredientToDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };

            // Act
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
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);

            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await dbContext.Ingredients.AddAsync(ingredient);
            await dbContext.SaveChangesAsync();

            var updatedIngredient = new Ingredient() { IngredientId = 1, Name = "Updated name", Description = "Updated description", Unit = "Updated unit", Package = "Updated package"};

            // Act
            await ingredientService.UpdateExistingIngredientAsync(ingredient, updatedIngredient);
            await dbContext.SaveChangesAsync();

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
            var ingredientService = new IngredientService(dbContext, _databaseErrorHandlerMock.Object);
            var ingredient = new Ingredient() { IngredientId = 1, Name = "Test name", Description = "Test description", Unit = "Test unit", Package = "Test package", Stock = 10, SafetyStock = 5 };
            await dbContext.Ingredients.AddAsync(ingredient);
            await dbContext.SaveChangesAsync();

            // Act
            await ingredientService.DeleteIngredientAsync(ingredient);

            // Assert
            var ingredientFromDb = await dbContext.Ingredients.FindAsync(ingredient.IngredientId);
            ingredientFromDb.Should().BeNull();
        }
    }
}