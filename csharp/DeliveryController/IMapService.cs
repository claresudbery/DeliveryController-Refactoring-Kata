using System;

namespace DeliveryController;

public interface IMapService
{
    double CalculateEta(Location location1, Location location2);
    void UpdateAverageSpeed(Location location1, Location location2, TimeSpan elapsedTime);
}