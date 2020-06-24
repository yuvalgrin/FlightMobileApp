using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FlightMobileWeb.Models;
using Microsoft.Extensions.Configuration;

namespace FlightMobileApp.Models
{
    public class SimulatorConnector : ISimulatorConnector
    {
        private string _hostIp;
        private int _httpPort;
        private SimTcpClient _simTcpClient;
        private bool _isConnected = false;
        private ConcurrentDictionary<string, double> _lastCommands;

        public SimulatorConnector(IConfiguration configuration)
        {
            // Load the configuration from launchSettings.json
            _hostIp = configuration.GetValue<string>("FlightSimulator:Host");
            _httpPort = configuration.GetValue<int>("FlightSimulator:Port.Http");
            int socketPort = configuration.GetValue<int>("FlightSimulator:Port.Socket");

            _simTcpClient = new SimTcpClient(_hostIp, socketPort);
            _isConnected = _simTcpClient.InitializeConnection();
            _lastCommands = new ConcurrentDictionary<string, double>();
        }

        /* Send commands to simulator return false if got error in simulator */
        public bool SendCommand(FlightCommand flightCommand)
        {
            if (!_isConnected)
            {
                if (_isConnected = _simTcpClient.InitializeConnection())
                    return false;
            }

            List<Command> commands = ExtractValidCommands(flightCommand);
            foreach (Command commad in commands)
            {
                bool res = _simTcpClient.RunCommandAndVerify(commad);
                if (!res)
                    return false;
            }
            return true;
        }

        /* Create the commands to send to the simulator */
        private List<Command> ExtractValidCommands(FlightCommand flightCommand)
        {
            List<Command> validCommands = new List<Command>();
            if (isNewCommand("aileron", flightCommand.Aileron))
                validCommands.Add(new Command("/controls/flight/aileron", flightCommand.Aileron));

            if (isNewCommand("elevator", flightCommand.Elevator))
                validCommands.Add(new Command("/controls/flight/elevator", flightCommand.Elevator));

            if (isNewCommand("rudder", flightCommand.Rudder))
                validCommands.Add(new Command("/controls/flight/rudder", flightCommand.Rudder));

            if (isNewCommand("throttle", flightCommand.Throttle))
                validCommands.Add(new Command("/controls/engines/current-engine/throttle",
                    flightCommand.Throttle));

            return validCommands;
        }

        /* Check if we try to send new value or and existing one */
        private bool isNewCommand(string type, double value)
        {
            _lastCommands.TryGetValue(type, out Double oldValue);
            if (oldValue != value)
            {
                _lastCommands[type] = value;
                return true;
            }
            return false;
        }

        /* Get and parse image from the simulator api */
        public Byte[] GetScreenshot()
        {
            try
            {
                string reqUrl = "http://" + _hostIp + ":" + _httpPort + "/screenshot";
                Task<Byte[]> task = ExecuteAsyncGet(reqUrl);
                return task.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /* Get http async request */
        public static async Task<Byte[]> ExecuteAsyncGet(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(1);
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

    }
}
