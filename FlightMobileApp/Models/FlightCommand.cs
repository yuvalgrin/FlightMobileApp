using System;
using Newtonsoft.Json;
namespace FlightMobileApp.Models
{
    public class FlightCommand
    {
        public FlightCommand()
        {
        }

        public FlightCommand(double aileron, double elevator,
            double rudder, double throttle)
        {
            this.Aileron = aileron;
            this.Elevator = elevator;
            this.Rudder = rudder;
            this.Throttle = throttle;
        }

        [JsonProperty("aileron")]
        public double Aileron { get; set; }
        [JsonProperty("elevator")]
        public double Elevator { get; set; }
        [JsonProperty("rudder")]
        public double Rudder { get; set; }
        [JsonProperty("throttle")]
        public double Throttle { get; set; }
    }
}
