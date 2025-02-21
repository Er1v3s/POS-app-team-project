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
    }
}