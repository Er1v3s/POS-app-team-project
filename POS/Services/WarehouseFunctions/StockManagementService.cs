using DataAccess.Models;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.Services.WarehouseFunctions
{
    public class StockManagementService
    {
        private readonly IngredientService _ingredientService;

        public StockManagementService(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        public void EditIngredientQuantity(Ingredient ingredient)
        {
            var stockCorrection = new StockCorrectionWindow(ingredient);
            var dialogResult = stockCorrection.ShowDialog();

            if (dialogResult == true)
                _ingredientService.GetAllIngredients();
        }
    }
}
