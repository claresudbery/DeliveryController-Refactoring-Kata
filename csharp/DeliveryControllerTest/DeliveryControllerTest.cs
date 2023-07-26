using System;
using System.Collections.Generic;
using System.Globalization;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest: IMapService
    {
        private readonly Delivery _salmonDelivery01;
        private readonly Delivery _salmonDelivery02;
        private readonly List<Delivery> _testDeliverySchedule;
        private readonly DeliveryEvent _salmonDeliveryEvent01;
        private readonly DeliveryEvent _salmonDeliveryEvent02;
        private readonly DateTime _salmonDeliveryTime01 = new DateTime(2023, 12, 31, 14, 05, 0);
        private readonly DateTime _salmonDeliveryTime02 = new DateTime(2023, 12, 31, 15, 07, 0);
        private readonly Location _salmonDeliveryLocation = new Location((float)12.2, (float)13.3);
        private const string SalmonDeliveryId01 = "SalmonDelivery01";
        private const string SalmonDeliveryId02 = "SalmonDelivery02";
        private bool _averageSpeedUpdated = false;

        public DeliveryControllerTest()
        {
            _salmonDelivery01 = new Delivery(
                id: SalmonDeliveryId01,
                contactEmail: "Sally@Sally.co.uk",
                location: _salmonDeliveryLocation,
                timeOfDelivery: _salmonDeliveryTime01,
                arrived: false,
                onTime: false);
            _salmonDelivery02 = new Delivery(
                id: SalmonDeliveryId02,
                contactEmail: "Sally@Sally.co.uk",
                location: _salmonDeliveryLocation,
                timeOfDelivery: _salmonDeliveryTime02,
                arrived: false,
                onTime: false);
            _testDeliverySchedule = new List<Delivery> { _salmonDelivery01, _salmonDelivery02 };
            _salmonDeliveryEvent01 = new DeliveryEvent(
                id: SalmonDeliveryId01, 
                timeOfDelivery: _salmonDeliveryTime01, 
                location: _salmonDeliveryLocation);
            _salmonDeliveryEvent02 = new DeliveryEvent(
                id: SalmonDeliveryId02, 
                timeOfDelivery: _salmonDeliveryTime02, 
                location: _salmonDeliveryLocation);
        }

        private DeliveryEvent MakeSalmonDeliveryEvent01(DateTime timeOfDelivery)
        {
            return new DeliveryEvent(
                id: SalmonDeliveryId01, 
                timeOfDelivery: timeOfDelivery, 
                location: _salmonDeliveryLocation);
        }

        private DeliveryEvent MakeSalmonDeliveryEvent02(DateTime timeOfDelivery)
        {
            return new DeliveryEvent(
                id: SalmonDeliveryId02, 
                timeOfDelivery: timeOfDelivery, 
                location: _salmonDeliveryLocation);
        }

        [Fact]
        public void Test_UpdateDelivery_UpdatesDelivery_ToArrived()
        {
            // Arrange
            var stubEmailGateway = new MockEmailGateway();
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                stubEmailGateway, 
                new MapService());
            _salmonDelivery01.Arrived = false;
            var initialArrivedVal = _salmonDelivery01.Arrived;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent01);
            
            // Assert
            Assert.True(_salmonDelivery01.Arrived);
            Assert.NotEqual(_salmonDelivery01.Arrived, initialArrivedVal);
        }

        [Fact]
        public void Test_UpdateDelivery_SetsOnTimeToTrue_IfDeliveryWithinTenMins()
        {
            // Arrange
            var stubEmailGateway = new MockEmailGateway();
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                stubEmailGateway, 
                new MapService());
            var deliveryTime = _salmonDeliveryTime01.AddMinutes(7);
            _salmonDelivery01.OnTime = false;
            var initialOnTimeVal = _salmonDelivery01.OnTime;
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent01(deliveryTime));
            
            // Assert
            Assert.True(_salmonDelivery01.OnTime);
            Assert.NotEqual(_salmonDelivery01.OnTime, initialOnTimeVal);
        }

        [Fact]
        public void Test_UpdateDelivery_SetsOnTimeToFalse_IfDeliveryAfterTenMins()
        {
            // Arrange
            var stubEmailGateway = new MockEmailGateway();
            var dummyMapService = this;
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                stubEmailGateway, 
                dummyMapService);
            var deliveryTime = _salmonDeliveryTime01.AddMinutes(11);
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent01(deliveryTime));
            
            // Assert
            Assert.False(_salmonDelivery01.OnTime);
        }

        [Fact]
        public void Test_UpdateDelivery_SendsEmail()
        {
            // Arrange
            MockEmailGateway spyEmailGateway = new MockEmailGateway();
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                spyEmailGateway, 
                new MapService());
            spyEmailGateway.Reset();
            var initialEmailStatus = spyEmailGateway.EmailSent;
            var dummyDeliveryEvent = MakeSalmonDeliveryEvent01(_salmonDeliveryTime01);
            
            // Act
            controller.UpdateDelivery(dummyDeliveryEvent);
            
            // Assert
            Assert.True(spyEmailGateway.EmailSent);
            Assert.NotEqual(spyEmailGateway.EmailSent, initialEmailStatus);
        }

        [Fact]
        public void Test_UpdateDelivery_DoesNotUpdateAverageSpeed_WhenDeliveryOnTime()
        {
            // Arrange
            var dummyEmailGateway = new MockEmailGateway();
            var spyMapService = this;
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                dummyEmailGateway, 
                spyMapService);
            _averageSpeedUpdated = false;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent01);
            
            // Assert
            Assert.False(_averageSpeedUpdated);
        }

        [Fact]
        public void Test_UpdateDelivery_DoesNotUpdateAverageSpeed_WhenDeliveryOnTime_AndMultipleDeliveries()
        {
            // Arrange
            var dummyEmailGateway = new MockEmailGateway();
            var spyMapService = this;
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                dummyEmailGateway, 
                spyMapService);
            _averageSpeedUpdated = false;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent02);
            
            // Assert
            Assert.False(_averageSpeedUpdated);
        }

        [Fact]
        public void Test_UpdateDelivery_UpdatesAverageSpeed_WhenDeliveryLate_AndMultipleDeliveries()
        {
            // Arrange
            var dummyEmailGateway = new MockEmailGateway();
            var spyMapService = this;
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                dummyEmailGateway, 
                spyMapService);
            _averageSpeedUpdated = false;
            var initialSpeedStatus = _averageSpeedUpdated;
            
            var deliveryTime02 = _salmonDeliveryTime02.AddMinutes(11);
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent02(deliveryTime02));
            
            // Assert
            Assert.True(_averageSpeedUpdated);
            Assert.NotEqual(_averageSpeedUpdated, initialSpeedStatus);
        }

        [Fact]
        public void Test_UpdateDelivery_SendsEmail_ReNextDelivery_WhenMultipleDeliveries()
        {
            // Arrange
            MockEmailGateway spyEmailGateway = new MockEmailGateway();
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                spyEmailGateway, 
                new StubMapService());
            spyEmailGateway.Reset();
            var initialEmailStatus = spyEmailGateway.EmailSent;
            
            // Act
            controller.UpdateDelivery(_salmonDeliveryEvent01);
            
            // Assert
            Assert.True(spyEmailGateway.EmailSent);
            Assert.NotEqual(spyEmailGateway.EmailSent, initialEmailStatus);
            Assert.Contains(
                StubMapService.StubEta.ToString(CultureInfo.InvariantCulture),
                spyEmailGateway.EmailMessage);
        }

        public double CalculateEta(Location location1, Location location2)
        {
            return 0;
        }

        void IMapService.UpdateAverageSpeed(Location location1, Location location2, TimeSpan elapsedTime)
        {
            _averageSpeedUpdated = true;
        }
    }
}