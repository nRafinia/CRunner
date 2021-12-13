using CRunner.Models;
using System.Net.Sockets;

namespace CRunner.Providers;

public class TelnetService : IProvider
{
    private string _ip;
    private TcpClient _client;
    private Security _security;
    private readonly Logger _logger;

    public TelnetService(Logger logger)
    {
        _logger = logger;
    }

    public void Connect(string ip, Security security)
    {
        _ip = ip;
        _client = new TcpClient(ip, 23);
        _security = security;
    }

    public async Task Run(IEnumerable<string> commands)
    {
        if (_client.Connected)
        {
            _logger.WriteLineGreen($"Connected to {_ip}");
            _logger.WriteLine("");
        }
        else
        {
            _logger.WriteLineMagenta("Could not connect, disconnected.");
            _logger.WriteLine("");
            return;
        }

        var stream = _client.GetStream();

        //var strReader = new StreamReader(stream);
        var str = ReadMessage(stream);
        _logger.WriteGray(str);

        await Task.Delay(1000);

        str = ReadMessage(stream);
        _logger.WriteGray(str);

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
            _logger.WriteGray(str);
        }

        _client.Close();

        _logger.WriteLineMagenta("");
        _logger.WriteLineMagenta($"Disconnected from {_ip}");

        await Task.Delay(1000);

    }

    public static string ReadMessage(NetworkStream stream)
    {
        if (!stream.DataAvailable)
            return string.Empty;

        // Receive response
        var responseData = new byte[256];
        var numberOfBytesRead = stream.Read(responseData, 0, responseData.Length);
        var response = System.Text.Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);
        return response;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}