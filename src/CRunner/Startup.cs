using CRunner.Models;
using CRunner.Providers;

namespace CRunner;

public class Startup
{
    private readonly IServiceProvider _provider;
    private readonly RunSetting _setting;
    private readonly Logger _logger;

    public Startup(IServiceProvider provider, RunSetting setting, Logger logger)
    {
        _provider = provider;
        _setting = setting;
        _logger = logger;
    }

    public async Task Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        _logger.WriteLineGray("Starting...");
        _logger.WriteLine("");

        await Run();

        _logger.WriteLineGray("Finished.");

        _logger.WriteLineGray("");
        _logger.WriteGray("Press Enter to exit....");
        Console.ReadLine();
    }

    private async Task Run()
    {
        var ips = _setting.Address.IP;
        foreach (var ip in ips)
        {
            var security = ip.Value?.Security ?? _setting.Security;
            var config = ip.Value ?? new IpConfig(security, ConnectMode.Ssh);
            await RunForIp(ip.Key, config, security);
        }
    }

    private async Task RunForIp(string ip, IpConfig config, Security security)
    {
        var commandProvider = GetCommandProvider(config);
        try
        {
            _logger.WriteLineGray($"Connecting to {ip} with {config.Mode}...");
            var isConnected = commandProvider.Connect(ip, security);

            if (!isConnected)
            {
                _logger.WriteLineMagenta("Could not connect, disconnected.");
                _logger.WriteLine("");
                commandProvider?.Dispose();
                return;
            }

            _logger.WriteLineGreen("Connected.");
            _logger.WriteLine("");

            await commandProvider.Run(_setting.Commands.Lines);

            commandProvider.Disconnect();
            _logger.WriteLineMagenta("");
            _logger.WriteLineMagenta($"Disconnected from {ip}");

        }
        catch (Exception e)
        {
            _logger.WriteLine("");
            _logger.WriteLineDarkRed($"Error in connect to {ip}. Error={e.Message}");
        }
        finally
        {
            commandProvider?.Dispose();
        }

        await Task.Delay(1000);
        _logger.WriteLine("");

    }

    private IProvider GetCommandProvider(IpConfig config)
    {
        var provider = config.Mode == ConnectMode.Ssh
            ? typeof(SshService)
            : typeof(TelnetService);
        return _provider.GetService(provider) as IProvider;
    }

}