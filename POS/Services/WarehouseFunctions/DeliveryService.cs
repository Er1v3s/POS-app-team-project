using System.Collections.ObjectModel;
using POS.Models.Warehouse;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using POS.Utilities;
using System.Windows;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.Services.WarehouseFunctions
{
    public class DeliveryService
    {
        private readonly IngredientService _ingredientService;
        private readonly GenerateDeliveryService _generateDeliveryService;

        public MyObservableCollection<DeliveryDto> DeliveryCollection;

        public DeliveryService(IngredientService ingredientService, GenerateDeliveryService generateDeliveryService)
        {
            _ingredientService = ingredientService;
            _generateDeliveryService = generateDeliveryService;

            DeliveryCollection = new();
        }

        public void AddIngredientToDeliveryCollection(Ingredient selectedIngredient)
        {
            var existingIngredient = DeliveryCollection.FirstOrDefault(i => i.Ingredient.IngredientId == selectedIngredient.IngredientId);

            if (existingIngredient == null)
            {
                var deliveryItem = new DeliveryDto
                {
                    Ingredient = selectedIngredient,
                    Quantity = 1
                };

                DeliveryCollection.Add(deliveryItem);
            }
            else
                existingIngredient.Quantity++;
        }

        public void IncreaseIngredientQuantity(DeliveryDto deliveryItem)
        {
            deliveryItem.Quantity++;
        }

        public void EditIngredientQuantity(Ingredient ingredient)
        {
            var stockCorrection = new StockCorrectionWindow(ingredient);
            var dialogResult = stockCorrection.ShowDialog();

            if (dialogResult == true)
                _ingredientService.GetAllIngredients();
        }

        public void DeleteIngredientFromDeliveryCollection(Ingredient ingredient)
        {
            var existingIngredient = DeliveryCollection.FirstOrDefault(i => i.Ingredient.IngredientId == ingredient.IngredientId);

            if (existingIngredient!.Quantity == 1)
            {
                var result = MessageBox.Show("Czy usunąć składnik z listy całkowicie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    DeliveryCollection.Remove(existingIngredient);
            }
            else
                existingIngredient.Quantity--;
        }

        public void CancelDelivery()
        {
            DeliveryCollection.Clear();
        }

        public async Task GenerateDeliveryDocument()
        {
            var deliveryItemList = DeliveryCollection.ToList();

            var result = await _generateDeliveryService.GenerateDeliveryDocument(deliveryItemList);

            if(result)
                CancelDelivery();
        }
    }
}
