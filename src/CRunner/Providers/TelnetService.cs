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

    public bool Connect(string ip, Security security)
    {
        _ip = ip;
        _security = security;
        try
        {
            _client = new TcpClient(ip, 23);
        }
        catch
        {
            //
        }

        return _client?.Connected ?? false;
    }

    public async Task Run(IEnumerable<string> commands)
    {
        var stream = _client.GetStream();

        var str = ReadMessage(stream);
        _logger.WriteGray(str);

        await Task.Delay(1000);

        await SendAuthorize(stream);

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
    }

    public void Disconnect()
    {
        _client.Close();
    }

    private static string ReadMessage(NetworkStream stream)
    {
        if (!stream.DataAvailable)
            return string.Empty;

        // Receive response
        var responseData = new byte[256];
        var numberOfBytesRead = stream.Read(responseData, 0, responseData.Length);
        var response = System.Text.Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);
        return response;
    }

    private async Task SendAuthorize(NetworkStream stream)
    {
        var cmd = System.Text.Encoding.ASCII.GetBytes(_security.UserName);
        await stream.WriteAsync(cmd, 0, cmd.Length);
        stream.WriteByte(13);
        await stream.FlushAsync();
        await Task.Delay(100);

        cmd = System.Text.Encoding.ASCII.GetBytes(_security.Password);
        await stream.WriteAsync(cmd, 0, cmd.Length);
        stream.WriteByte(13);
        await stream.FlushAsync();
        await Task.Delay(100);

        var str = ReadMessage(stream);
        _logger.WriteGray(str);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}