﻿using DataAccess.Models;
using FluentAssertions;
using POS.Models.Warehouse;
using POS.Services.WarehouseFunctions;

namespace POS.Tests.UnitTests.WarehouseFunctions
{
    public class DeliveryServiceUnitTests
    {
        [Fact]
        public void DeliveryService_OnServiceInitialize_DeliveryCollectionShouldNotBeNull()
        {
            // Act
            var deliveryService = new DeliveryService();

            // Assert
            deliveryService.DeliveryCollection.Should().NotBeNull();
        }

        [Fact]
        public void IncreaseIngredientQuantityInDelivery_ForExistingIngredient_ShouldIncreaseQuantityByOne()
        {
            // Arrange
            var deliveryService = new DeliveryService();
            var deliveryItem = new DeliveryDto
            {
                Quantity = 1,
                Ingredient = new Ingredient() { Name = "Whisky", Description = "Jack Daniel's", Unit = "szt", Package = "Szklana butelka 700ml", Stock = 10, SafetyStock = 5 },
            };
            deliveryService.DeliveryCollection.Add(deliveryItem);

            // Act
            deliveryService.IncreaseIngredientQuantityInDelivery(deliveryItem);

            // Assert
            deliveryService.DeliveryCollection.Should().HaveCount(1);
            deliveryService.DeliveryCollection[0].Quantity.Should().Be(2);
        }

        
    }
}
