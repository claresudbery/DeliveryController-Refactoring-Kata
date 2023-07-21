using System;
using System.Collections.Generic;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest
    {
        [Fact]
        public void Test_UpdateDelivery_UpdatesDelivery_ToArrived()
        {
            // Arrange
            var salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
            var salmonDeliveryId = "SalmonDelivery01";
            var timeOfDelivery = new DateTime(2023, 12, 31, 14, 05, 0);
            var salmonDelivery = new Delivery(
                id: salmonDeliveryId,
                contactEmail: "Sally@Sally.co.uk",
                location: salmonDeliveryLocation,
                timeOfDelivery: timeOfDelivery,
                arrived: false,
                onTime: false);
            List<Delivery> testDeliverySchedule = new List<Delivery>{salmonDelivery};
            var controller = new DeliveryController.DeliveryController(
                testDeliverySchedule);
            
            // Act
            var deliveryEvent = new DeliveryEvent(
                id: salmonDeliveryId, 
                timeOfDelivery: timeOfDelivery, 
                location: salmonDeliveryLocation);
            controller.UpdateDelivery(deliveryEvent);
            
            // Assert
            Assert.True(salmonDelivery.Arrived);
        }
    }
}