using System;
using System.Collections.Generic;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest: IEmailGateway, IMapService
    {
        private readonly Delivery _salmonDelivery;
        private readonly List<Delivery> _testDeliverySchedule;
        private readonly DeliveryEvent _salmonDeliveryEvent;
        private readonly DateTime _salmonDeliveryTime = new DateTime(2023, 12, 31);
        private readonly Location _salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
        private const string SalmonDeliveryId = "SalmonDelivery01";
        private bool _emailSent = false;
        private bool _averageSpeedUpdated = false;

        public DeliveryControllerTest()
        {
            _salmonDelivery = new Delivery(
                id: SalmonDeliveryId,
                contactEmail: "Sally@Sally.co.uk",
                location: _salmonDeliveryLocation,
                timeOfDelivery: _salmonDeliveryTime,
                arrived: false,
                onTime: false);
            _testDeliverySchedule = new List<Delivery> { _salmonDelivery };
            _salmonDeliveryEvent = new DeliveryEvent(
                id: SalmonDeliveryId, 
                timeOfDelivery: _salmonDeliveryTime, 
                location: _salmonDeliveryLocation);
        }

        private DeliveryEvent MakeSalmonDeliveryEvent(DateTime timeOfDelivery)
        {
            return new DeliveryEvent(
                id: SalmonDeliveryId, 
                timeOfDelivery: timeOfDelivery, 
                location: _salmonDeliveryLocation);
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
            var deliveryTime = _salmonDeliveryTime.AddMinutes(7);
            _salmonDelivery.OnTime = false;
            var initialOnTimeVal = _salmonDelivery.OnTime;
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent(deliveryTime));
            
            // Assert
            Assert.True(_salmonDelivery.OnTime);
            Assert.NotEqual(_salmonDelivery.OnTime, initialOnTimeVal);
        }

        [Fact]
        public void Test_UpdateDelivery_SetsOnTimeToFalse_IfDeliveryAfterTenMins()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                this, 
                new MapService());
            var deliveryTime = _salmonDeliveryTime.AddMinutes(11);
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent(deliveryTime));
            
            // Assert
            Assert.False(_salmonDelivery.OnTime);
        }

        [Fact]
        public void Test_UpdateDelivery_SendsEmail()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                this, 
                new MapService());
            _emailSent = false;
            var initialEmailStatus = _emailSent;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent);
            
            // Assert
            Assert.True(_emailSent);
            Assert.NotEqual(_emailSent, initialEmailStatus);
        }

        [Fact]
        public void Test_UpdateDelivery_DoesNotUpdateAverageSpeed_WhenDeliveryOnTime()
        {
            // Arrange
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                this, 
                new MapService());
            _averageSpeedUpdated = false;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent);
            
            // Assert
            Assert.False(_averageSpeedUpdated);
        }

        void IEmailGateway.Send(string address, string subject, string message)
        {
            _emailSent = true;
        }

        public double CalculateEta(Location location1, Location location2)
        {
            // Do nothing
            return 0;
        }

        void IMapService.UpdateAverageSpeed(Location location1, Location location2, TimeSpan elapsedTime)
        {
            _averageSpeedUpdated = true;
        }
    }
}