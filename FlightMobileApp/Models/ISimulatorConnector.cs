using System;
using System.Drawing;

namespace FlightMobileApp.Models
{
    public interface ISimulatorConnector
    {
        bool SendCommand(FlightCommand flightCommand);
        Byte[] GetScreenshot();
    }
}
