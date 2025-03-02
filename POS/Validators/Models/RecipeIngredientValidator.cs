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
                .WithMessage("Quantity cannot be negative")
                .LessThanOrEqualTo(1000)
                .WithMessage("Quantity cannot be higher than 1000")
                .Must(HaveTwoDecimalPlacesOrLess)
                .WithMessage("The value must have up to two decimal places");
        }

        public ValidationResult ValidateQuantity(string quantity)
        {
            if (string.IsNullOrWhiteSpace(quantity))
                return new ValidationResult(false, "Property \"quantity\" cannot be empty");
            if (!double.TryParse(quantity.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double quantityAsDouble))
                return new ValidationResult(false, "Property \"quantity\" must be a numeric value");
            if (quantityAsDouble < 0 || quantityAsDouble > 1000)
                return new ValidationResult(false, "Property \"quantity\" must be between 0 and 1000");
            if (!HaveTwoDecimalPlacesOrLess(quantityAsDouble))
                return new ValidationResult(false, "Property \"quantity\" can have up to two decimal places");

            return new ValidationResult(true);
        }

        private bool HaveTwoDecimalPlacesOrLess(double quantity)
        {
            string quantityAsString = quantity.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            string[] parts = quantityAsString.Split('.');

            if (parts.Length == 2 && parts[1].Length > 2)
                return false;

            return true;
        }
    }
}
