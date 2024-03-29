﻿using CRunner.Models;
using Renci.SshNet;

namespace CRunner.Providers;

public class SshService : IProvider
{
    private SshClient _client;
    private readonly ILogger _logger;
    private readonly ModuleService _moduleService;

    public SshService(ILogger logger, ModuleService moduleService)
    {
        _logger = logger;
        _moduleService = moduleService;
    }

    public bool Connect(string ip, Security security)
    {
        var connectionInfo = new ConnectionInfo(ip,
            22,
            security.UserName,
            new PasswordAuthenticationMethod(security.UserName, security.Password)
            /*new PrivateKeyAuthenticationMethod("rsa.key")*/);
        try
        {
            _client = new SshClient(connectionInfo);
            _client.Connect();
        }
        catch
        {
            //
        }

        return _client?.IsConnected ?? false;
    }

    public async Task Run(IEnumerable<string> commands)
    {
        var stream = _client.CreateShellStream("CRunner", 0, 0, 0, 0, 1024);

        var commandRunner = new SshCommandRunner(stream);

        stream.DataReceived += (sender, eventArgs) =>
        {
            var data = System.Text.Encoding.UTF8.GetString(eventArgs.Data);
            _logger.WriteGray(data);
        };

        await Task.Delay(1000);

        foreach (var cmd in commands)
        {
            var commandItems = cmd.Split(' ');

            if (_moduleService.Exist(commandItems[0]))
            {
                var commandHandler = _moduleService.Get(commandItems[0]);
                await commandHandler.RunCommand(commandItems[1..]);
                continue;
            }

            await commandRunner.Run(cmd);
        }

    }

    public void Disconnect()
    {
        _client.Disconnect();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

}