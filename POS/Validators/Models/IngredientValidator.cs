using DataAccess.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace POS.Validators.Models
{
    public class IngredientValidator : AbstractValidator<Ingredient>
    {
        public IngredientValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .Must(BeValidName)
                .WithMessage("Name can only contains letters, numbers, and spaces")
                .MaximumLength(100)
                .WithMessage("Name should not be longer than 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty")
                .Must(BeValidDescription)
                .WithMessage("Description can only contains letters, numbers, and spaces")
                .MaximumLength(400)
                .WithMessage("Description should not be longer than 400 characters");

            RuleFor(x => x.Unit)
                .NotEmpty()
                .WithMessage("Unit cannot be empty")
                .Must(BeValidUnit)
                .WithMessage("Unit can only contains letters, and spaces")
                .MaximumLength(100)
                .WithMessage("Unit should not exceed 100 characters");

            RuleFor(x => x.Package)
                .NotEmpty()
                .WithMessage("Package cannot be empty")
                .Must(BeValidPackage)
                .WithMessage("Package can only contains letters, and spaces")
                .MaximumLength(100)
                .WithMessage("Package should not exceed 100 characters");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative")
                .LessThan(1000)
                .WithMessage("Stock cannot be higher than 1000");
                

            RuleFor(x => x.SafetyStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("SafetyStock cannot be negative")
                .LessThan(1000)
                .WithMessage("SafetyStock cannot be higher than 1000");
        }

        public ValidationResult ValidateIngredientName(string ingredientName)
        {
            if (string.IsNullOrWhiteSpace(ingredientName))
                return new ValidationResult(false, "Name cannot be empty");
            if (!BeValidName(ingredientName))
                return new ValidationResult(false, "Name can only contains letters, numbers, and spaces");
            if (ingredientName.Length < 3)
                return new ValidationResult(false, "Ingredient name must be at least 3 characters long.");
            if (ingredientName.Length > 100)
                return new ValidationResult(false, "Ingredient name must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        public ValidationResult ValidateIngredientUnit(string ingredientUnit)
        {
            if (string.IsNullOrWhiteSpace(ingredientUnit))
                return new ValidationResult(false, "Property \"unit\" cannot be empty");
            if (!BeValidPackage(ingredientUnit))
                return new ValidationResult(false, "Property \"unit\" can only contains letters, and spaces");
            if (ingredientUnit.Length > 100)
                return new ValidationResult(false, "Property \"unit\" must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        public ValidationResult ValidateIngredientPackage(string ingredientPackage)
        {
            if (string.IsNullOrWhiteSpace(ingredientPackage))
                return new ValidationResult(false, "Property \"package\" cannot be empty");
            if (!BeValidPackage(ingredientPackage))
                return new ValidationResult(false, "Property \"package\" can only contains letters, numbers, and spaces");
            if (ingredientPackage.Length > 100)
                return new ValidationResult(false, "Property \"package\" must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        public ValidationResult ValidateIngredientDescription(string ingredientDescription)
        {
            if (string.IsNullOrWhiteSpace(ingredientDescription))
                return new ValidationResult(false, "Property \"description\" cannot be empty");
            if (!BeValidDescription(ingredientDescription))
                return new ValidationResult(false, "Property \"description\" can only contains letters, numbers, and spaces");
            if (ingredientDescription.Length < 5)
                return new ValidationResult(false, "Property \"description\" must be at least 5 characters long.");
            if (ingredientDescription.Length > 100)
                return new ValidationResult(false, "Property \"description\" must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        private bool BeValidName(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s]*$");
        }

        private bool BeValidDescription(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s()'"".,!?%:@#&+\-/*]*$");
        }

        private bool BeValidUnit(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s]*$");
        }

        private bool BeValidPackage(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s]*$");
        }
    }
}
