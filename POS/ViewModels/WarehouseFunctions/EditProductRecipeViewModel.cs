using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class EditProductRecipeViewModel : ViewModelBase
    {
        private string amountOfIngredientInRecipe;

        public string AmountOfIngredientInRecipe
        {
            get => amountOfIngredientInRecipe;
            set => SetField(ref amountOfIngredientInRecipe, value);
        }
    }
}
