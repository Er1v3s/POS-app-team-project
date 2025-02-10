using System;

namespace POS.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message = "Brak wyników wyszukiwania") : base(message) { }
    }
}
