using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using POS.Exceptions.Interfaces;

namespace POS.Exceptions
{
    public class DatabaseErrorHandler : IDatabaseErrorHandler
    {
        private readonly ILogger<DatabaseErrorHandler> _logger;

        public DatabaseErrorHandler(ILogger<DatabaseErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteDatabaseOperationAsync(Func<Task> action, Action? onFailure = null)
        {
            try
            {
                await action();
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                onFailure?.Invoke();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Błąd zapisu w bazie danych.");
                ExceptionsHandler.ShowErrorMessage("Wystąpił problem z zapisaniem danych. Spróbuj ponownie później.");
                onFailure?.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Błąd połączenia z bazą danych.");
                ExceptionsHandler.ShowErrorMessage("Nie można połączyć się z bazą danych.");
                onFailure?.Invoke();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Przekroczono czas operacji.");
                ExceptionsHandler.ShowErrorMessage("Operacja trwała zbyt długo. Spróbuj ponownie.");
                onFailure?.Invoke();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operacja zapisu do bazy danych została anulowana.");
                ExceptionsHandler.ShowErrorMessage("Operacja zapisu do bazy danych nie powiodła się. Spróbuj ponownie.");
                onFailure?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd.");
                ExceptionsHandler.ShowErrorMessage("Wystąpił nieoczekiwany problem.");
                onFailure?.Invoke();
            }
        }

        public async Task<T> ExecuteDatabaseOperationAsync<T>(Func<Task<T>> action, Action? onFailure = null)
        {
            try
            {
                return await action();
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                onFailure?.Invoke();
                return default!;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Błąd zapisu w bazie danych.");
                ExceptionsHandler.ShowErrorMessage("Wystąpił problem z zapisaniem danych. Spróbuj ponownie później.");
                onFailure?.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Błąd połączenia z bazą danych.");
                ExceptionsHandler.ShowErrorMessage("Nie można połączyć się z bazą danych.");
                onFailure?.Invoke();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Przekroczono czas operacji.");
                ExceptionsHandler.ShowErrorMessage("Operacja trwała zbyt długo. Spróbuj ponownie.");
                onFailure?.Invoke();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operacja zapisu do bazy danych została anulowana.");
                ExceptionsHandler.ShowErrorMessage("Operacja zapisu do bazy danych nie powiodła się. Spróbuj ponownie.");
                onFailure?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd.");
                ExceptionsHandler.ShowErrorMessage("Wystąpił nieoczekiwany problem.");
                onFailure?.Invoke();
            }

            return default;
        }
    }
}
