using DataAccess.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace POS.Validators.Models
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .Must(BeValidName)
                .WithMessage("Name can only contains letters, numbers, and spaces")
                .MaximumLength(100)
                .WithMessage("Name should not be longer than 100 characters");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category cannot be empty")
                .Must(BeValidName)
                .WithMessage("Category can only contains letters, and spaces")
                .MaximumLength(100)
                .WithMessage("Category should not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty")
                .Must(BeValidDescription)
                .WithMessage("Description can only contains letters, numbers, and spaces")
                .MaximumLength(400)
                .WithMessage("Description should not be longer than 400 characters");

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price cannot be empty")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be negative");
        }

        public ValidationResult ValidateProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new ValidationResult(false, "Name cannot be empty");
            if (!BeValidName(name))
                return new ValidationResult(false, "Name can only contains letters, numbers, and spaces");
            if (name.Length < 3)
                return new ValidationResult(false, "Name must be at least 3 characters long.");
            if (name.Length > 100)
                return new ValidationResult(false, "Name must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        public ValidationResult ValidateProductCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new ValidationResult(false, "Property \"category\" cannot be empty");
            if (!BeValidName(category))
                return new ValidationResult(false, "Property \"category\" can only contains letters, numbers, and spaces");
            if (category.Length > 100)
                return new ValidationResult(false, "Property \"category\" must be less than 100 characters long.");

            return new ValidationResult(true);
        }

        public ValidationResult ValidateProductDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return new ValidationResult(false, "Property \"description\" cannot be empty");
            if (!BeValidDescription(description))
                return new ValidationResult(false, "Property \"description\" can only contains letters, numbers, and spaces");
            if (description.Length < 5)
                return new ValidationResult(false, "Property \"description\" must be at least 5 characters long.");
            if (description.Length > 400)
                return new ValidationResult(false, "Property \"description\" must be less than 400 characters long.");

            return new ValidationResult(true);
        }

        private bool BeValidName(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s]*$");
        }

        private bool BeValidDescription(string text)
        {
            return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s()',.!?%]*$");
        }

        //private bool BeValidUnit(string text)
        //{
        //    return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s]*$");
        //}

        //private bool BeValidPackage(string text)
        //{
        //    return Regex.IsMatch(text, @"^[a-za-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9\s]*$");
        //}
    }
}
