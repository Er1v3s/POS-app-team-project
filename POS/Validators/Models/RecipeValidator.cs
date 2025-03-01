using DataAccess.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace POS.Validators.Models
{
    public class RecipeValidator : AbstractValidator<Recipe>
    {
        public RecipeValidator()
        {
            RuleFor(x => x.RecipeName)
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .Must(BeValidName)
                .WithMessage("Name can only contains letters, numbers, and spaces")
                .MaximumLength(200)
                .WithMessage("Name should not be longer than 200 characters");

            RuleFor(x => x.RecipeContent)
                .NotEmpty()
                .WithMessage("Recipe cannot be empty")
                .Must(BeValidContent)
                .WithMessage("Recipe can only contains letters, numbers, and spaces")
                .MinimumLength(5)
                .WithMessage("Recipe must be at least 5 characters long.")
                .MaximumLength(1000)
                .WithMessage("Recipe should not be longer than 1000 characters");
        }

        public ValidationResult ValidateRecipeContent(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return new ValidationResult(false, "Property \"description\" cannot be empty");
            if (!BeValidContent(description))
                return new ValidationResult(false, "Property \"description\" can only contains letters, numbers, and spaces");
            if (description.Length < 5)
                return new ValidationResult(false, "Property \"description\" must be at least 5 characters long.");
            if (description.Length > 1000)
                return new ValidationResult(false, "Property \"description\" must be less than 1000 characters long.");

            return new ValidationResult(true);
        }

        private bool BeValidName(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s]*$");
        }

        private bool BeValidContent(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s()'"".,!?%:@#&+\-/*]*$");
        }
    }
}
