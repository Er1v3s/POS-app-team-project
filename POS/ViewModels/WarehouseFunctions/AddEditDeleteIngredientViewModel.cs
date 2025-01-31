using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteIngredientViewModel : ViewModelBase
    {
        private string ingredientName;
        private string ingredientUnit;
        private string ingredientPackage;
        private string ingredientDescription;

        public string IngredientName
        {
            get => ingredientName;
            set => SetField(ref ingredientName, value);
        }

        public string IngredientUnit
        {
            get => ingredientUnit;
            set => SetField(ref ingredientUnit, value);
        }

        public string IngredientPackage
        {
            get => ingredientPackage;
            set => SetField(ref ingredientPackage, value);
        }

        public string IngredientDescription
        {
            get => ingredientDescription;
            set => SetField(ref ingredientDescription, value);
        }
    }
}
