using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileWeb.Models
{
    public class Command
    {
        public Command(string key, double value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public double Value { get; set; }
    }
}
