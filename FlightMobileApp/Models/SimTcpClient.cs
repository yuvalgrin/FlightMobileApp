using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Text;
using System.Globalization;

public class SimTcpClient
{
    private const string OnInitCommand = "data\n ";
    public const string ERR = "ERR";

    private string _hostIp { get; set; }
    private int _portSocket { get; set; }
    private NetworkStream _stream;

    public SimTcpClient(string ip, int portSocket) { 
        this._hostIp = ip;
        this._portSocket = portSocket;
    }


    public bool InitializeConnection()
    {
        try
        {
            TcpClient tcpClient = new TcpClient(_hostIp, _portSocket);
            _stream = tcpClient.GetStream();

            string resp = RunCommand(OnInitCommand);

            if (ERR.Equals(resp))
                return false;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public string RunCommand(string command)
    {
        try
        {
            if (_stream == null)
                return string.Empty;

            Byte[] data = null;
            // Get a client stream for reading and writing.
            // Translate the passed message into ASCII and store it as a Byte array.
            data = Encoding.ASCII.GetBytes(command);

            // Send the message to the connected TcpServer. 
            _stream.Write(data, 0, data.Length);
            data = new Byte[256];

            // String to store the response ASCII representation.
            string responseData = string.Empty;

            // Read the first batch of the TcpServer response bytes.
            Thread.Sleep(30);
            _stream.ReadTimeout = 15000;
            Int32 bytes = _stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            responseData = responseData.Substring(0, responseData.Length - 1);
            return responseData;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

}