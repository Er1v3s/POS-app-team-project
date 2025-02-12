using Serilog;
using System;

namespace POS.Exceptions
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger = Log.Logger;

        public void LogInformation(string message) => _logger.Information(message);

        public void LogError(Exception ex, string message) => _logger.Error(ex, message);
    }
}
