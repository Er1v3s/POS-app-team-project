using System;
using System.Threading.Tasks;

namespace POS.Exceptions.Interfaces
{
    public interface IDatabaseErrorHandler
    {
        Task ExecuteDatabaseOperationAsync(Func<Task> operation, Action? onFailure = null);
        Task<T> ExecuteDatabaseOperationAsync<T>(Func<Task<T>> operation, Action? onFailure = null);
    }
}
