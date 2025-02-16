using DataAccess.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests
{
    public class IngredientValidatorTests
    {
        private readonly IngredientValidator _validator;

        public IngredientValidatorTests()
        {
            _validator = new IngredientValidator();
        }

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { "abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~" };
            yield return new object[] { new string('A', 101) };
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void Name_Should_Have_Error_For_Invalid_Values(string invalidName)
        {
            // Arrange
            var ingredient = new Ingredient()
            {
                Name = invalidName,
                Description = "Valid",
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void Name_Should_Have_Error_For_Invalid_Values_2(string invalidName)
        {
            var isIngredientNameValid = _validator.ValidateIngredientName(invalidName);

            isIngredientNameValid.Result.Should().BeFalse();
            isIngredientNameValid.ErrorMessage.Should().NotBe("");
            isIngredientNameValid.ErrorMessage.Should().NotBeEmpty();
            isIngredientNameValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return new object[] { "ValidName" };
            yield return new object[] { "Valid Name 123" };
            yield return new object[] { "ValidName123" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void Name_Should_Not_Have_Error_For_Valid_Values(string validName)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = validName,
                Description = "Valid",
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void Name_Should_Not_Have_Error_For_Valid_Values_2(string validName)
        {
            var isIngredientNameValid = _validator.ValidateIngredientName(validName);

            isIngredientNameValid.Result.Should().BeTrue();
            isIngredientNameValid.ErrorMessage.Should().Be(null);
        }

        public static IEnumerable<object[]> InvalidDescription()
        {
            yield return new object[] { "" };
            yield return new object[] { "`~@#$^&*-_=+[]{};:<>|" };
            yield return new object[] { new string('A', 401) };
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void Description_Should_Have_Error_For_Invalid_Values(string invalidDescription)
        {
            // Arrange
            var ingredient = new Ingredient()
            {
                Name = "Valid Name",
                Description = invalidDescription,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void Description_Should_Have_Error_For_Invalid_Values_2(string invalidDescription)
        {
            var isIngredientDescriptionValid = _validator.ValidateIngredientDescription(invalidDescription);

            isIngredientDescriptionValid.Result.Should().BeFalse();
            isIngredientDescriptionValid.ErrorMessage.Should().NotBe("");
            isIngredientDescriptionValid.ErrorMessage.Should().NotBeEmpty();
            isIngredientDescriptionValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> ValidDescription()
        {
            yield return new object[] { "ValidDescription 123 !.,()" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void Description_Should_Not_Have_Error_For_Valid_Values(string validDescription)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid Name",
                Description = validDescription,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void Description_Should_Not_Have_Error_For_Valid_Values_2(string validDescription)
        {
            var isIngredientDescriptionValid = _validator.ValidateIngredientDescription(validDescription);

            isIngredientDescriptionValid.Result.Should().BeTrue();
            isIngredientDescriptionValid.ErrorMessage.Should().Be(null);
        }

        public static IEnumerable<object[]> InvalidValues()
        {
            yield return new object[] { "" };
            yield return new object[] { "InvalidValue123" };
            yield return new object[] { "InvalidValue!@#$%^&*()" };
            yield return new object[] { new string('A', 101) };
        }

        public static IEnumerable<object[]> ValidValues()
        {
            yield return new object[] { "ValidValue" };
            yield return new object[] { "Valid Value" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void Unit_Should_Have_Error_For_Invalid_Values(string validUnit)
        {
            // Arrange

            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Unit = validUnit,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Unit);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void Unit_Should_Have_Error_For_Invalid_Values_2(string invalidUnit)
        {
            var isIngredientUnitValid = _validator.ValidateIngredientUnit(invalidUnit);

            isIngredientUnitValid.Result.Should().BeFalse();
            isIngredientUnitValid.ErrorMessage.Should().NotBe("");
            isIngredientUnitValid.ErrorMessage.Should().NotBeEmpty();
            isIngredientUnitValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void Unit_Should_Not_Have_Error_For_Valid_Values(string validUnits)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Unit = validUnits,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Unit);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void Unit_Should_Not_Have_Error_For_Valid_Values_2(string validUnit)
        {
            var isIngredientUnitValid = _validator.ValidateIngredientUnit(validUnit);

            isIngredientUnitValid.Result.Should().BeTrue();
            isIngredientUnitValid.ErrorMessage.Should().Be(null);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void Package_Should_Have_Error_For_Invalid_Values(string invalidPackage)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Package = invalidPackage,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Package);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void Package_Should_Have_Error_For_Invalid_Values_2(string invalidUnits)
        {
            var isIngredientPackageValid = _validator.ValidateIngredientPackage(invalidUnits);

            isIngredientPackageValid.Result.Should().BeFalse();
            isIngredientPackageValid.ErrorMessage.Should().NotBe("");
            isIngredientPackageValid.ErrorMessage.Should().NotBeEmpty();
            isIngredientPackageValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void Package_Should_Not_Have_Error_For_Valid_Values(string validUnits)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Package = validUnits,
                Stock = 10,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Package);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void Package_Should_Not_Have_Error_For_Valid_Values_2(string validUnit)
        {
            var isIngredientPackageValid = _validator.ValidateIngredientDescription(validUnit);

            isIngredientPackageValid.Result.Should().BeTrue();
            isIngredientPackageValid.ErrorMessage.Should().Be(null);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void Stock_Should_Have_Error_For_Invalid_Values(int invalidStockValue)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Stock = invalidStockValue,
                SafetyStock = 5,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Stock);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void SafetyStock_Should_Have_Error_For_Invalid_Values(int invalidSafetyStockValue)
        {
            // Arrange
            var ingredient = new Ingredient
            {
                Name = "Valid 123",
                Description = "Valid",
                Stock = 5,
                SafetyStock = invalidSafetyStockValue,
            };

            // Act
            var result = _validator.TestValidate(ingredient);

            // Arrange
            result.ShouldHaveValidationErrorFor(x => x.SafetyStock);
        }
    }
}