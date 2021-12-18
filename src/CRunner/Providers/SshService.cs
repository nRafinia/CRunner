using CRunner.Models;
using Renci.SshNet;

namespace CRunner.Providers;

public class SshService : IProvider
{
    private SshClient _client;
    private readonly Logger _logger;
    private readonly CommandService _commandService;

    public SshService(Logger logger, CommandService commandService)
    {
        _logger = logger;
        _commandService = commandService;
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
        var stream = _client.CreateShellStream("CRunner", 0, 0, 0, 0, 10);//1024);

        var commandRunner = new SshCommandRunner(stream);

        stream.DataReceived += (sender, eventArgs) =>
        {
            var data = System.Text.Encoding.UTF8.GetString(eventArgs.Data);
            _logger.WriteGray(data);
        };

        await Task.Delay(1000);

        foreach (var cmd in commands)
        {
            var commandItems = cmd.Split(" ");

            if (_commandService.Exist(commandItems[0]))
            {
                var commandHandler = _commandService.Get(commandItems[0]);
                await commandHandler.GetCommand(commandRunner, commandItems[1..], _logger);
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