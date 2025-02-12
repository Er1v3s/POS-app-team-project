using System;

namespace POS.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Brak wyników wyszukiwania") : base(message) { }
    }
}
