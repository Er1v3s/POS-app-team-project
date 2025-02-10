using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace POS.Exceptions
{
    public class DatabaseErrorHandler
    {
        private readonly ILogger<DatabaseErrorHandler> _logger;

        public DatabaseErrorHandler(ILogger<DatabaseErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteDatabaseOperationAsync(Func<Task> action, Action<Exception> onFailure = null)
        {
            try
            {
                await action();
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                onFailure?.Invoke(ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Błąd zapisu w bazie danych.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Wystąpił problem z zapisaniem danych. Spróbuj ponownie później.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Błąd połączenia z bazą danych.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Nie można połączyć się z bazą danych.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Przekroczono czas operacji.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Operacja trwała zbyt długo. Spróbuj ponownie.");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operacja zapisu do bazy danych została anulowana.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Operacja zapisu do bazy danych nie powiodła się. Spróbuj ponownie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Wystąpił nieoczekiwany problem.");
            }
        }

        public async Task<T> ExecuteDatabaseOperationAsync<T>(Func<Task<T>> action, Action<Exception> onFailure = null)
        {
            try
            {
                return await action();
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                onFailure?.Invoke(ex);
                return default!;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Błąd zapisu w bazie danych.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Wystąpił problem z zapisaniem danych. Spróbuj ponownie później.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Błąd połączenia z bazą danych.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Nie można połączyć się z bazą danych.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Przekroczono czas operacji.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Operacja trwała zbyt długo. Spróbuj ponownie.");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operacja zapisu do bazy danych została anulowana.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Operacja zapisu do bazy danych nie powiodła się. Spróbuj ponownie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd.");
                onFailure?.Invoke(ex);
                throw new DatabaseException("Wystąpił nieoczekiwany problem.");
            }
        }
    }
}
