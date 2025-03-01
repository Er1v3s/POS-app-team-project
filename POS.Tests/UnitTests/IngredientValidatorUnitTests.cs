using DataAccess.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class IngredientValidatorUnitTests
    {
        private readonly IngredientValidator _validator;

        public IngredientValidatorUnitTests()
        {
            _validator = new IngredientValidator();
        }

        private readonly Ingredient ingredient = new ()
        {
            Name = "Test Name",
            Description = "Test description",
            Unit = "Test unit",
            Package = "Test package",
            Stock = 10,
            SafetyStock = 5,
        };

        #region Validate Ingredient Name

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { "abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~" };
            yield return new object[] { new string('A', 101) };
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return new object[] { "ValidName" };
            yield return new object[] { "Valid Name 123" };
            yield return new object[] { "ValidName123" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void IngredientName_ForInvalidValue_ShouldHaveValidationError(string invalidName)
        {
            // Arrange
            ingredient.Name = invalidName;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void NameValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidName)
        {
            var isIngredientNameValid = _validator.ValidateIngredientName(invalidName);

            isIngredientNameValid.Result.Should().BeFalse();
            isIngredientNameValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void IngredientName_ForValidValue_ShouldNotHaveValidationError(string validName)
        {
            // Arrange
            ingredient.Name = validName;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void NameValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validName)
        {
            // Act
            var isIngredientNameValid = _validator.ValidateIngredientName(validName);

            // Assert
            isIngredientNameValid.Result.Should().BeTrue();
            isIngredientNameValid.ErrorMessage.Should().BeNull();
        }

        #endregion

        #region Validate Ingredient Description

        public static IEnumerable<object[]> InvalidDescription()
        {
            yield return new object[] { "" };
            yield return new object[] { "`~@#$^&*-_=+[]{};:<>|" };
            yield return new object[] { new string('A', 401) };
        }

        public static IEnumerable<object[]> ValidDescription()
        {
            yield return new object[] { "ValidDescription 123 !.,()" };
            yield return new object[] { new string('A', 100) };
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void IngredientDescription_ForInvalidValues_ShouldHaveValidationError(string invalidDescription)
        {
            // Arrange
            ingredient.Description = invalidDescription;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void IngredientDescriptionValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidDescription)
        {
            // Act
            var isIngredientDescriptionValid = _validator.ValidateIngredientDescription(invalidDescription);

            // Assert
            isIngredientDescriptionValid.Result.Should().BeFalse();
            isIngredientDescriptionValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void IngredientDescription_ForValidValue_ShouldNotHaveValidationError(string validDescription)
        {
            // Arrange
            ingredient.Description = validDescription;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void IngredientDescriptionValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validDescription)
        {
            // Act
            var isIngredientDescriptionValid = _validator.ValidateIngredientDescription(validDescription);

            // Assert
            isIngredientDescriptionValid.Result.Should().BeTrue();
            isIngredientDescriptionValid.ErrorMessage.Should().BeNull();
        }

        #endregion

        #region Validate Ingredient Unit and Package

        public static IEnumerable<object[]> InvalidValues()
        {
            yield return new object[] { "" };
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
        public void IngredientUnit_ForInvalidValues_ShouldHaveValidationError(string validUnit)
        {
            // Arrange
            ingredient.Unit = validUnit;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Unit);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void IngredientUnitValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidUnit)
        {
            // Act
            var isIngredientUnitValid = _validator.ValidateIngredientUnit(invalidUnit);

            // Assert
            isIngredientUnitValid.Result.Should().BeFalse();
            isIngredientUnitValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void IngredientUnit_ForValidValue_ShouldNotHaveValidationError(string validUnits)
        {
            // Arrange
            ingredient.Unit = validUnits;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Unit);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void IngredientUnitValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validUnit)
        {
            // Act
            var isIngredientUnitValid = _validator.ValidateIngredientUnit(validUnit);

            // Assert
            isIngredientUnitValid.Result.Should().BeTrue();
            isIngredientUnitValid.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void IngredientPackage_ForInvalidValues_ShouldHaveValidationError(string invalidValue)
        {
            // Arrange
            ingredient.Package = invalidValue;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Package);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void IngredientPackageValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidValue)
        {
            // Act
            var isIngredientPackageValid = _validator.ValidateIngredientPackage(invalidValue);

            // Assert
            isIngredientPackageValid.Result.Should().BeFalse();
            isIngredientPackageValid.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void IngredientPackage_ForValidValue_ShouldNotHaveValidationError(string validValue)
        {
            // Arrange
            ingredient.Package = validValue;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Package);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void IngredientPackageValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validValue)
        {
            // Act
            var isIngredientPackageValid = _validator.ValidateIngredientDescription(validValue);

            // Assert
            isIngredientPackageValid.Result.Should().BeTrue();
            isIngredientPackageValid.ErrorMessage.Should().BeNull();
        }

        #endregion

        #region Validate Ingredient Stock and Safety Stock

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void IngredientStock_ForInvalidValue_ShouldHaveValidationError(int invalidValue)
        {
            // Arrange
            ingredient.Stock = invalidValue;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Stock);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void IngredientSafetyStock_ForInvalidValue_ShouldHaveValidationError(int invalidValue)
        {
            // Arrange
            ingredient.SafetyStock = invalidValue;

            // Act
            var result = _validator.TestValidate(ingredient);

            // Arrange
            result.ShouldHaveValidationErrorFor(x => x.SafetyStock);
        }

        #endregion
    }
}