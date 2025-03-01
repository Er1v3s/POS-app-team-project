using System;

namespace POS.Exceptions.Logger
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
    }
}
