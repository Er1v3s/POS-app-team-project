using Microsoft.IdentityModel.Tokens;

namespace POS.Validators
{
    public class ValidationResult
    {
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }

        public ValidationResult(bool result, string? errorMessage = null)
        {
            Result = result;

            if (errorMessage.IsNullOrEmpty())
            {
                ErrorMessage = Result == false ? "Validation failed." : null;
            }
            else
            {
                ErrorMessage = errorMessage;
            }
        }
    }
}
