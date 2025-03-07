using System;
using DataAccess.Models;
using POS.Models.Warehouse;
using POS.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace POS.Services.WarehouseFunctions
{
    public class DeliveryService
    {
        private readonly GenerateDeliveryService _generateDeliveryService;

        public MyObservableCollection<DeliveryDto> DeliveryCollection;

        public event Action DeliveryCollectionUpdated;

        public DeliveryService(GenerateDeliveryService generateDeliveryService)
        {
            _generateDeliveryService = generateDeliveryService;

            DeliveryCollection = new();
        }

        public void IncreaseIngredientQuantityInDelivery(DeliveryDto deliveryItem)
        {
            deliveryItem.Quantity++;
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

            DeliveryCollectionUpdated?.Invoke();
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

            if (result)
                CancelDelivery();
        }
    }
}
