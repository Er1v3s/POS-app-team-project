using DataAccess.Models;
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

        
    }
}
