using System;
using System.Collections.Generic;
using DeliveryController;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest
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
        private bool _emailSent = false;
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
            var controller = new DeliveryController.DeliveryController(
                _testDeliverySchedule, 
                new EmailGateway(), 
                new MapService());
            _salmonDelivery01.Arrived = false;
            
            // Act
            controller.UpdateDelivery(MakeSalmonDeliveryEvent01(_salmonDeliveryTime01));
            
            // Assert
            Assert.True(_salmonDelivery01.Arrived);
        }
    }
}