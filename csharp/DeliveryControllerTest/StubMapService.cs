using System;
using DeliveryController;

namespace DeliveryControllerTest;

public class StubMapService : IMapService
{
    public static double StubEta = 23.46;
        
    public double CalculateEta(Location location1, Location location2)
    {
        return StubEta;
    }

    public void UpdateAverageSpeed(Location location1, Location location2, TimeSpan elapsedTime)
    {
    }
}