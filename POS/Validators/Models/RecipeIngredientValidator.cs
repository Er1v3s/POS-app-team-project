using DataAccess.Models;
using FluentValidation;
using System.Globalization;

namespace POS.Validators.Models
{
    public class RecipeIngredientValidator : AbstractValidator<RecipeIngredient>
    {
        public RecipeIngredientValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative")
                .LessThan(1000)
                .WithMessage("Stock cannot be higher than 1000");
        }

        public ValidationResult ValidateQuantity(string quantity)
        {
            if (string.IsNullOrWhiteSpace(quantity))
                return new ValidationResult(false, "Property \"quantity\" cannot be empty");
            if (!double.TryParse(quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out double priceAsDouble))
                return new ValidationResult(false, "Property \"quantity\" must be a numeric value");
            if (priceAsDouble < 0 || priceAsDouble > 10000)
                return new ValidationResult(false, "Property \"quantity\" must be between 0 and 10000");

            string[] parts = quantity.Split('.');
            if (parts.Length == 2 && parts[1].Length > 2)
                return new ValidationResult(false, "Property \"quantity\" can have up to two decimal places");

            return new ValidationResult(true);
        }
    }
}
