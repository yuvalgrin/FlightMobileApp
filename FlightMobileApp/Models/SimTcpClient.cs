using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Text;
using System.Globalization;
using FlightMobileWeb.Models;

public class SimTcpClient
{
    private const string OnInitCommand = "data\n ";
    public const string ERR = "ERR";
    static object Lock = new object();
    static object LockInit = new object();

    private string _hostIp { get; set; }
    private int _portSocket { get; set; }
    private NetworkStream _stream;

    public SimTcpClient(string ip, int portSocket)
    {
        this._hostIp = ip;
        this._portSocket = portSocket;
    }


    public bool InitializeConnection()
    {
        try
        {
            lock (LockInit)
            {
                TcpClient tcpClient = new TcpClient(_hostIp, _portSocket);
                _stream = tcpClient.GetStream();

                string resp = RunCommand(OnInitCommand, false);

                if (ERR.Equals(resp))
                    return false;

                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }


    public bool RunCommandAndVerify(Command command)
    {
        var res = RunCommand("set " + command.Key + " " + command.Value + " \n", false);
        if (res == null)
            return false;

        string simValue = RunCommand("get " + command.Key + " \n", true);
        if (simValue == null)
            return false;
        try
        {
            double diff = Math.Abs(double.Parse(simValue) - command.Value);
            // Verify simulator has the same value with 1% diff
            if (diff < 0.01)
                return true;
        }
        catch (Exception)
        {
            return false;
        }
        return false;
    }

    public string RunCommand(string command, bool readResp)
    {
        try
        {
            lock (Lock)
            {
                if (_stream == null)
                    return null;

                Byte[] data = null;
                // Get a client stream for reading and writing.
                // Translate the passed message into ASCII and store it as a Byte array.
                data = Encoding.ASCII.GetBytes(command);

                // Send the message to the connected TcpServer. 
                _stream.Write(data, 0, data.Length);
                data = new Byte[256];

                // String to store the response ASCII representation.
                string responseData = string.Empty;

                if (readResp)
                {
                    // Read the first batch of the TcpServer response bytes.
                    Thread.Sleep(30);
                    _stream.ReadTimeout = 100;
                    Int32 bytes = _stream.Read(data, 0, data.Length);
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    responseData = responseData.Substring(0, responseData.Length - 1);
                    return responseData;
                }
                return string.Empty;
            }
        }
        catch (IOException)
        {
            // Occurs when failed reading - when response is empty
            return string.Empty;
        }
        catch (SocketException)
        {
            return null;
        }


    }

}