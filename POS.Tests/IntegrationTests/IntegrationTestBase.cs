using DataAccess;
using Microsoft.EntityFrameworkCore;
using Moq;
using POS.Exceptions.Interfaces;

namespace POS.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected readonly Mock<IDatabaseErrorHandler> _databaseErrorHandlerMock;
        protected readonly AppDbContext _dbContext;

        protected IntegrationTestBase(string inMemoryDbName)
        {
            _databaseErrorHandlerMock = new Mock<IDatabaseErrorHandler>();

            _databaseErrorHandlerMock
                .Setup(x => x.ExecuteDatabaseOperationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()))
                .Returns<Func<Task>, Action<Exception>>((operation, onFailure) => operation());

            _dbContext = GetInMemoryDbContext(inMemoryDbName);
        }

        protected AppDbContext GetInMemoryDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var dbContext = new AppDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            SeedDatabase(dbContext).Wait();
            return dbContext;
        }

        protected abstract Task SeedDatabase(AppDbContext dbContext);
    }
}
