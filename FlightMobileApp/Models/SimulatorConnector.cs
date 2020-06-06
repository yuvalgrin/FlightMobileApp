using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FlightMobileApp.Models
{
    public class SimulatorConnector
    {
        private string _hostIp;
        private int _httpPort;
        private SimTcpClient _simTcpClient;
        private bool _isConnected = false;

        public SimulatorConnector(IConfiguration configuration)
        {
            _hostIp = configuration.GetValue<string>("FlightSim:Host");
            _httpPort = configuration.GetValue<int>("FlightSim:Port.Http");
            int socketPort = configuration.GetValue<int>("FlightSim:Port.Socket");

            _simTcpClient = new SimTcpClient(_hostIp, socketPort);
            _isConnected = _simTcpClient.InitializeConnection();
        }

        public bool SendCommand(FlightCommand flightCommand)
        {
            if (!_isConnected)
            {
                if (_isConnected = _simTcpClient.InitializeConnection())
                    return false;
            }

            List<string> commands = ExtractValidCommands(flightCommand);
            foreach (string commad in commands) {
                if (_simTcpClient.RunCommand(commad) == string.Empty)
                    return false;
            }
            return true;
        }

        private List<string> ExtractValidCommands(FlightCommand flightCommand)
        {
            List<string> validCommands = new List<string>();

            validCommands.Add("set /controls/flight/aileron " + flightCommand.Aileron + " \\n");
            validCommands.Add("set /controls/flight/elevator " + flightCommand.Elevator + " \\n");
            validCommands.Add("set /controls/flight/rudder " + flightCommand.Rudder + " \\n");
            validCommands.Add("set /controls/flight/throttle " + flightCommand.Throttle + " \\n");

            return validCommands;
        }

        public Byte[] GetScreenshot()
        {
            string reqUrl = "https://" + _hostIp + ":" + _httpPort + "/screenshot";
            Task<Byte[]> task = ExecuteAsyncGet(reqUrl);
            return task.Result;
        }

        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }

        /** Get http async request */
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
