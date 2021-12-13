using CRunner.Models;
using Renci.SshNet;

namespace CRunner.Providers;

public class SshService : IProvider
{
    private string _ip;
    private SshClient _client;
    private Logger _logger;

    public SshService(Logger logger)
    {
        _logger = logger;
    }

    public void Connect(string ip, Security security)
    {
        _ip = ip;
        var connectionInfo = new ConnectionInfo(ip,
            22,
            security.UserName,
            new PasswordAuthenticationMethod(security.UserName, security.Password)
            /*new PrivateKeyAuthenticationMethod("rsa.key")*/);

        _client = new SshClient(connectionInfo);
        _client.Connect();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task Run(IEnumerable<string> commands)
    {
        if (_client.IsConnected)
        {
            _logger.WriteLineGreen($"Connected to {_ip}");
            _logger.WriteLine("");
        }
        else
        {
            _logger.WriteLineMagenta("Could not connect, disconnected.");
            return;
        }

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
                //await Task.Delay(int.Parse(sleep[1]));
                var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(int.Parse(sleep[1])));
                await periodicTimer.WaitForNextTickAsync();
                periodicTimer.Dispose();
                continue;
            }

            //stream.WriteLine(cmd);
            foreach (var c in cmd)
            {
                stream.WriteByte((byte)c);
                await stream.FlushAsync();
            }
            stream.WriteByte(13);
            await stream.FlushAsync();
        }

        _client.Disconnect();

        _logger.WriteLineMagenta("");
        _logger.WriteLineMagenta($"Disconnected from {_ip}");
    }
}