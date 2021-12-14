using CRunner.Models;
using Renci.SshNet;

namespace CRunner.Providers;

public class SshService : IProvider
{
    private SshClient _client;
    private readonly Logger _logger;

    public SshService(Logger logger)
    {
        _logger = logger;
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
        var stream = _client.CreateShellStream("Customcommand", 0, 0, 0, 0, 10);//1024);

        stream.DataReceived += (sender, eventArgs) =>
        {
            var data = System.Text.Encoding.UTF8.GetString(eventArgs.Data);
            _logger.WriteGray(data);
        };

        await Task.Delay(1000);

        foreach (var cmd in commands)
        {
            if (cmd.StartsWith("sleep"))
            {
                var sleep = cmd.Split(":");
                var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(int.Parse(sleep[1])));
                await periodicTimer.WaitForNextTickAsync();
                periodicTimer.Dispose();
                continue;
            }

            foreach (var c in cmd)
            {
                stream.WriteByte((byte)c);
                await stream.FlushAsync();
            }
            stream.WriteByte(13);
            await stream.FlushAsync();
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