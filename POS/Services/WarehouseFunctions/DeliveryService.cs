using System;
using DataAccess.Models;
using POS.Models.Warehouse;
using POS.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using POS.Handlers;

namespace POS.Services.WarehouseFunctions
{
    public class DeliveryService
    {
        public MyObservableCollection<DeliveryDto> DeliveryCollection;

        public event Action DeliveryCollectionUpdated;

        public DeliveryService()
        {
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
                DeliveryCollection.Remove(existingIngredient);
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

            var result = await DeliveryHandler.GenerateDeliveryDocument(deliveryItemList);

            if (result)
                DeliveryCollection.Clear();
        }
    }
}
