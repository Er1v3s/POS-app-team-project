﻿using System.Globalization;
using DataAccess.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class ProductValidatorUnitTests
    {
        private readonly ProductValidator _productValidator;

        public ProductValidatorUnitTests()
        {
            _productValidator = new ProductValidator();
        }

        private readonly Product product = new()
        {
            ProductName = "test name",
            Category = "test category",
            Description = "test description",
            Price = 25.99,
            Recipe = new Recipe() { RecipeName = "test name", RecipeContent = "test content" },
        };

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return [""];
            yield return ["abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~"];
            yield return [new string('A', 101)];
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return ["ValidName"];
            yield return ["Valid Name 123"];
            yield return ["ValidName123"];
            yield return [new string('A', 100)];
        }

        #region Validate Product Name

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void ProductName_ForInvalidValues_ShouldHaveValidationError(string invalidName)
        {
            // Arrange
            product.ProductName = invalidName;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.ProductName);
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void ProductName_ForValidValues_ShouldNotHaveValidationError(string validName)
        {
            // Arrange
            product.ProductName = validName;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldNotHaveValidationErrorFor(p => p.ProductName);
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void NameValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidName)
        {
            // Arrange
            product.ProductName = invalidName;

            // Act
            var validationResult = _productValidator.ValidateProductName(invalidName);

            // Assert
            validationResult.Result.Should().BeFalse();
            validationResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void NameValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validName)
        {
            // Act
            var validationResult = _productValidator.ValidateProductName(validName);

            // Assert
            validationResult.Result.Should().BeTrue();
            validationResult.ErrorMessage.Should().BeNullOrEmpty();
        }

        #endregion

        #region Validate Product Category

        public static IEnumerable<object[]> InvalidCategory()
        {
            yield return [""];
            yield return ["TestName123"];
            yield return ["TestName 123"];
            yield return ["`~@#$^&*-_=+[]{};:<>|"];
            yield return [new string('A', 401)];
        }

        public static IEnumerable<object[]> ValidCategory()
        {
            yield return ["ValidCategory"];
            yield return ["Valid Category"];
            yield return [new string('A', 100)];
        }

        [Theory]
        [MemberData(nameof(InvalidCategory))]
        public void ProductCategory_ForInvalidValues_ShouldHaveValidationError(string invalidCategory)
        {
            // Arrange
            product.Category = invalidCategory;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Theory]
        [MemberData(nameof(ValidCategory))]
        public void ProductCategory_ForValidValues_ShouldNotHaveValidationError(string validCategory)
        {
            // Arrange
            product.Category = validCategory;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Category);
        }

        [Theory]
        [MemberData(nameof(InvalidCategory))]
        public void CategoryValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(
            string invalidCategory)
        {
            // Arrange
            product.Category = invalidCategory;

            // Act
            var validationResult = _productValidator.ValidateProductCategory(invalidCategory);

            // Assert
            validationResult.Result.Should().BeFalse();
            validationResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [MemberData(nameof(ValidCategory))]
        public void CategoryValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validCategory)
        {
            // Act
            var validationResult = _productValidator.ValidateProductCategory(validCategory);

            // Assert
            validationResult.Result.Should().BeTrue();
            validationResult.ErrorMessage.Should().BeNullOrEmpty();
        }

        #endregion

        #region Validate Product Description

        public static IEnumerable<object[]> InvalidDescription()
        {
            yield return [""];
            yield return ["`~@#$^&*-_=+[]{};:<>|"];
            yield return [new string('A', 1001)];
        }

        public static IEnumerable<object[]> ValidDescription()
        {
            yield return ["ValidDescription 123 !.,()"];
            yield return [new string('A', 100)];
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void ProductDescription_ForInvalidValues_ShouldHaveValidationError(string invalidDescription)
        {
            // Arrange
            product.Description = invalidDescription;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void ProductDescription_ForValidValues_ShouldNotHaveValidationError(string validDescription)
        {
            // Arrange
            product.Description = validDescription;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [MemberData(nameof(InvalidDescription))]
        public void DescriptionValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(
            string invalidDescription)
        {
            // Act
            var validationResult = _productValidator.ValidateProductDescription(invalidDescription);

            // Assert
            validationResult.Result.Should().BeFalse();
            validationResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [MemberData(nameof(ValidDescription))]
        public void DescriptionValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(
            string validDescription)
        {
            // Act
            var validationResult = _productValidator.ValidateProductDescription(validDescription);

            // Assert
            validationResult.Result.Should().BeTrue();
            validationResult.ErrorMessage.Should().BeNullOrEmpty();
        }

        #endregion

        #region Validate Product Price

        [Theory]
        [InlineData("-1")]
        [InlineData("10001")]
        public void ProductPrice_ForInvalidValues_ShouldHaveValidationError(string invalidPrice)
        {
            // Arrange
            product.Price = double.Parse(invalidPrice, NumberStyles.Any, CultureInfo.InvariantCulture);

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Theory]
        [InlineData("20")]
        [InlineData("25.99")]
        [InlineData("31,99")]
        public void ProductPrice_ForValidValues_ShouldNotHaveValidationError(string validPrice)
        {
            // Arrange
            product.Price = double.Parse(validPrice, NumberStyles.Any, CultureInfo.InvariantCulture);

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("10001")]
        public void PriceValidationMethod_ForInvalidValues_ShouldReturnValidationResultEqualsFalse(string invalidPrice)
        {
            // Act
            var validationResult = _productValidator.ValidateProductPrice(invalidPrice);

            // Assert
            validationResult.Result.Should().BeFalse();
            validationResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("20")]
        [InlineData("25.99")]
        [InlineData("31,99")]
        public void PriceValidationMethod_ForValidValues_ShouldReturnValidationResultEqualsTrue(string validPrice)
        {
            // Act
            var validationResult = _productValidator.ValidateProductPrice(validPrice);

            // Assert
            validationResult.Result.Should().BeTrue();
            validationResult.ErrorMessage.Should().BeNullOrEmpty();
        }

        #endregion

        [Fact]
        public void RecipeProperty_ForNullValue_ShouldHaveValidationError()
        {
            // Arrange
            product.Recipe = null!;

            // Act
            var result = _productValidator.TestValidate(product);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Recipe);
        }
    }
}
