using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Exceptions
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
    }
}
