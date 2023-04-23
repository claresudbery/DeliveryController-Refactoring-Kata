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
            const string salmonDeliveryId = "SalmonDelivery01";
            var salmonDeliveryTime = new DateTime(2023, 12, 31);
            var salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
            var delivery = new Delivery(
                id: salmonDeliveryId,
                contactEmail: "Sally@Sally.co.uk",
                location: salmonDeliveryLocation,
                timeOfDelivery: salmonDeliveryTime,
                arrived: true,
                onTime: true);
            var deliverySchedule = new List<Delivery> { delivery };
            var controller = new DeliveryController.DeliveryController(deliverySchedule);
            var deliveryEvent = new DeliveryEvent(
                id: salmonDeliveryId, 
                timeOfDelivery: salmonDeliveryTime, 
                location: salmonDeliveryLocation);
            controller.UpdateDelivery(deliveryEvent);
            
            Assert.True(delivery.Arrived);
        }
    }
}