using Renci.SshNet;
using System.Net.Sockets;

namespace SSH.Providers.Telnet;

public class TelnetService
{
    private ConnectionInfo _connectionInfo;
    private string _ip;
    private TcpClient _client;

    public void Connect(string ip, string username, string password)
    {
        _ip = ip;
        _client = new TcpClient(ip, 23);
    }

    public async Task Run(string[] secs, string[] commands)
    {
        if (_client.Connected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Connected to {_ip}");
            Console.WriteLine("");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Could not connect, disconnected.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        var stream = _client.GetStream();

        //var strReader = new StreamReader(stream);
        var str = ReadMessage(stream);
        Console.Write(str);

        await Task.Delay(1000);
        foreach (var s in secs)
        {
            var cmd = System.Text.Encoding.ASCII.GetBytes(s);

            await stream.WriteAsync(cmd, 0, cmd.Length);
            stream.WriteByte(13);
            await stream.FlushAsync();
            await Task.Delay(100);
            str = ReadMessage(stream);
            Console.Write(str);
        }

        str = ReadMessage(stream);
        Console.Write(str);

        foreach (var cmd in commands)
        {
            if (cmd.StartsWith("sleep"))
            {
                var sleep = cmd.Split(":");
                await Task.Delay(int.Parse(sleep[1]));
                continue;
            }

            var cmdByte = System.Text.Encoding.ASCII.GetBytes(cmd);
            await stream.WriteAsync(cmdByte, 0, cmd.Length);
            stream.WriteByte(13);
            await stream.FlushAsync();
            await Task.Delay(100);
            str = ReadMessage(stream);
            Console.Write(str);
        }

        _client.Close();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        Console.WriteLine($"Disconnected from {_ip}");

        await Task.Delay(1000);

    }

    public string ReadMessage(NetworkStream stream)
    {
        if (!stream.DataAvailable)
            return string.Empty;

        // Receive response
        Byte[] responseData = new byte[256];
        Int32 numberOfBytesRead = stream.Read(responseData, 0, responseData.Length);
        string response = System.Text.Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);
        /*response = ParseData(response);
        if (response == "SEND_COMMAND_AGAIN")
        {
            if (DEBUG) Console.WriteLine("[ReadMessage] : Error Retreiving data. Send command again.");
        }*/
        return response;
    }
}