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
    }
}