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
                .MaximumLength(100)
                .WithMessage("Unit should not exceed 100 characters");

            RuleFor(x => x.Package)
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

        private bool BeValidName(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z0-9\s]+$");
        }

        private bool BeValidDescription(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z0-9\s(),.!?%]+$");
        }
    }
}
