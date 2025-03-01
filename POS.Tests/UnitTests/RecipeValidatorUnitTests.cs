using POS.Validators.Models;

namespace POS.Tests.UnitTests
{
    public class RecipeValidatorUnitTests
    {
        private readonly RecipeValidator _recipeValidator;

        public RecipeValidatorUnitTests()
        {
            _recipeValidator = new RecipeValidator();
        }

        #region Validate Recipe Name

        public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { "abcABC123!@#$%^&*()_-+={}|[];:'.<>/`~" };
            yield return new object[] { new string('A', 201) };
        }

        public static IEnumerable<object[]> ValidNames()
        {
            yield return new object[] { "ValidName" };
            yield return new object[] { "Valid Name 123" };
            yield return new object[] { "ValidName123" };
            yield return new object[] { new string('A', 200) };
        }

        #endregion
    }
}
