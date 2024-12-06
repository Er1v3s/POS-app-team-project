﻿namespace POS.Models.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string? ErrorMessage { get; }

        public ValidationResult(bool isValid, string? errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
