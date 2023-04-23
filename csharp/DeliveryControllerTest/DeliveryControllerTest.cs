using System;
using System.Collections.Generic;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest
    {
        private readonly Delivery _salmonDelivery;
        private readonly List<Delivery> _testDeliverySchedule;
        private readonly DeliveryEvent _salmonDeliveryEvent;
        private const string SalmonDeliveryId = "SalmonDelivery01";

        public DeliveryControllerTest()
        {
            var salmonDeliveryTime = new DateTime(2023, 12, 31);
            var salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
            _salmonDelivery = new Delivery(
                id: SalmonDeliveryId,
                contactEmail: "Sally@Sally.co.uk",
                location: salmonDeliveryLocation,
                timeOfDelivery: salmonDeliveryTime,
                arrived: false,
                onTime: false);
            _testDeliverySchedule = new List<Delivery> { _salmonDelivery };
            _salmonDeliveryEvent = new DeliveryEvent(
                id: SalmonDeliveryId, 
                timeOfDelivery: salmonDeliveryTime, 
                location: salmonDeliveryLocation);
        }

        [Fact]
        public void Test_UpdateDelivery_UpdatesDelivery_ToArrived()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                new EmailGateway(), 
                new MapService());
            _salmonDelivery.Arrived = false;
            var initialArrivedValue = _salmonDelivery.Arrived;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent);
            
            // Assert
            Assert.True(_salmonDelivery.Arrived);
            Assert.NotEqual(initialArrivedValue, _salmonDelivery.Arrived);
        }
    }
}