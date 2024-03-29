﻿using CRunner.Models;
using System.Net.Sockets;

namespace CRunner.Providers;

public class TelnetService : IProvider
{
    private TcpClient _client;
    private Security _security;
    private readonly ILogger _logger;
    private readonly ModuleService _moduleService;

    public TelnetService(ILogger logger, ModuleService moduleService)
    {
        _logger = logger;
        _moduleService = moduleService;
    }

    public bool Connect(string ip, Security security)
    {
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

        var commandRunner = new TelnetCommandRunner(stream);

        var str = ReadMessage(stream);
        _logger.WriteGray(str);

        await Task.Delay(1000);

        await SendAuthorize(stream);

        str = ReadMessage(stream);
        _logger.WriteGray(str);

        foreach (var cmd in commands)
        {
            var commandItems = cmd.Split(" ");

            if (_moduleService.Exist(commandItems[0]))
            {
                var commandHandler = _moduleService.Get(commandItems[0]);
                await commandHandler.RunCommand(commandItems[1..]);
                continue;
            }

            await commandRunner.Run(cmd);
            await Task.Delay(100);
            str = ReadMessage(stream);
            _logger.WriteGray(str);
        }
    }

    public void Disconnect()
    {
        _client.Close();
    }
    public void Dispose()
    {
        _client?.Dispose();
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

}