﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FlightMobileApp.Models
{
    public class SimulatorConnector : ISimulatorConnector
    {
        private string _hostIp;
        private int _httpPort;
        private SimTcpClient _simTcpClient;
        private bool _isConnected = false;

        public SimulatorConnector(IConfiguration configuration)
        {
            // Load the configuration from launchSettings.json
            _hostIp = configuration.GetValue<string>("FlightSimulator:Host");
            _httpPort = configuration.GetValue<int>("FlightSimulator:Port.Http");
            int socketPort = configuration.GetValue<int>("FlightSimulator:Port.Socket");

            _simTcpClient = new SimTcpClient(_hostIp, socketPort);
            _isConnected = _simTcpClient.InitializeConnection();
        }

        /* Send commands to simulator return false if got error in simulator */
        public bool SendCommand(FlightCommand flightCommand)
        {
            if (!_isConnected)
            {
                if (_isConnected = _simTcpClient.InitializeConnection())
                    return false;
            }

            List<string> commands = ExtractValidCommands(flightCommand);
            foreach (string commad in commands) {
                if (SimTcpClient.ERR.Equals(_simTcpClient.RunCommand(commad)))
                    return false;
            }
            return true;
        }

        /* Create the commands to send to the simulator */
        private List<string> ExtractValidCommands(FlightCommand flightCommand)
        {
            List<string> validCommands = new List<string>();

            validCommands.Add("set /controls/flight/aileron " + flightCommand.Aileron + " \n ");
            validCommands.Add("set /controls/flight/elevator " + flightCommand.Elevator + " \n ");
            validCommands.Add("set /controls/flight/rudder " + flightCommand.Rudder + " \n ");
            validCommands.Add("set /controls/engines/current-engine/throttle " + flightCommand.Throttle + " \n ");

            return validCommands;
        }

        /* Get and parse image from the simulator api */
        public Byte[] GetScreenshot()
        {
            try
            {
                string reqUrl = "http://" + _hostIp + ":" + _httpPort + "/screenshot";
                Task<Byte[]> task = ExecuteAsyncGet(reqUrl);
                return task.Result;
            } catch (Exception)
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
