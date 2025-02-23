using DataAccess;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions.Interfaces;
using POS.Services;

namespace POS.Tests.UnitTests
{
    public class IngredientServiceUnitTests
    {
        private readonly Mock<IDatabaseErrorHandler> _databaseErrorHandlerMock;
        private readonly Mock<AppDbContext> _appDbContextMock;

        public IngredientServiceUnitTests()
        {
            _databaseErrorHandlerMock = new Mock<IDatabaseErrorHandler>();

            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()))
                .Returns<Func<Task>, Action<Exception>>((operation, onFailure) => operation());

            _appDbContextMock = new Mock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>().Options);
        }

        [Theory]
        [InlineData("Test Name", "Test Description", "Test Unit", "Test Package")]
        [InlineData("TestName", "TestDescription", "TestUnit", "TestPackage")]
        [InlineData("Test Name123", "Test Description123", "Test Unit", "Test Package123")]
        [InlineData("TestName123", "TestDescription123',.!?%", "TestUnit", "TestPackage123")]
        public async Task CreateIngredient_CreateDataModelAndValidateIt_ReturnValidatedIngredientDataModel(string name, string description, string unit, string package)
        {
            // Arrange
            var ingredientService = new IngredientService(_appDbContextMock.Object, _databaseErrorHandlerMock.Object);

            // Act
            var newIngredient = await ingredientService.CreateIngredient(name, description, unit, package);

            // Assert
            newIngredient.Should().NotBeNull();
            newIngredient.Name.Should().Be(name);
            newIngredient.Description.Should().Be(description);
            newIngredient.Unit.Should().Be(unit);
            newIngredient.Package.Should().Be(package);
        }

        [Theory]
        [InlineData("Test Name !@#", "Test Description", "Test Unit", "Test Package")]
        [InlineData("Test Name", "Test Description !@#", "Test Unit", "Test Package")]
        [InlineData("Test Name", "Test Description !@#", "Test Unit !@#", "Test Package")]
        [InlineData("Test Name", "Test Description", "Test Unit", "Test Package !@#")]
        public async Task CreateIngredient_ForInvalidArguments_ThrowValidationException(string name, string description, string unit, string package)
        {
            // Arrange
            var ingredientService = new IngredientService(_appDbContextMock.Object, _databaseErrorHandlerMock.Object);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ValidationException>(async () => 
            {
                await ingredientService.CreateIngredient(name, description, unit, package);
            });
        }
    }
}
