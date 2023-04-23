using System;
using System.Collections.Generic;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest: IEmailGateway
    {
        private readonly Delivery _salmonDelivery;
        private readonly List<Delivery> _testDeliverySchedule;
        private readonly DeliveryEvent _salmonDeliveryEvent;
        private readonly DateTime _salmonDeliveryTime = new DateTime(2023, 12, 31);
        private const string SalmonDeliveryId = "SalmonDelivery01";

        public DeliveryControllerTest()
        {
            var salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
            _salmonDelivery = new Delivery(
                id: SalmonDeliveryId,
                contactEmail: "Sally@Sally.co.uk",
                location: salmonDeliveryLocation,
                timeOfDelivery: _salmonDeliveryTime,
                arrived: false,
                onTime: false);
            _testDeliverySchedule = new List<Delivery> { _salmonDelivery };
            _salmonDeliveryEvent = new DeliveryEvent(
                id: SalmonDeliveryId, 
                timeOfDelivery: _salmonDeliveryTime, 
                location: salmonDeliveryLocation);
        }

        [Fact]
        public void Test_UpdateDelivery_UpdatesDelivery_ToArrived()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                this, 
                new MapService());
            _salmonDelivery.Arrived = false;
            var initialArrivedVal = _salmonDelivery.Arrived;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent);
            
            // Assert
            Assert.True(_salmonDelivery.Arrived);
            Assert.NotEqual(_salmonDelivery.Arrived, initialArrivedVal);
        }

        [Fact]
        public void Test_UpdateDelivery_SetsOnTimeToTrue_IfDeliveryWithinTenMins()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                this, 
                new MapService());
            _salmonDelivery.OnTime = false;
            _salmonDelivery.TimeOfDelivery = _salmonDeliveryTime.AddMinutes(7);
            var initialOnTimeVal = _salmonDelivery.OnTime;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent);
            
            // Assert
            Assert.True(_salmonDelivery.OnTime);
            Assert.NotEqual(_salmonDelivery.OnTime, initialOnTimeVal);
        }

        void IEmailGateway.Send(string address, string subject, string message)
        {
            // Do nothing
        }
    }
}