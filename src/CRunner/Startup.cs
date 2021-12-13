using CRunner.Models;
using CRunner.Providers;
using Microsoft.Extensions.DependencyInjection;

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
            var security = ip.Value ?? _setting.Security;
            await RunForIp(ip.Key, security);
        }
    }

    private async Task RunForIp(string ip, Security security)
    {
        var commandProvider = GetCommandProvider();
        try
        {
            _logger.WriteLineGray($"Connecting to {ip}...");
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

    private IProvider GetCommandProvider()
    {
        return _provider.GetService<SshService>();
    }

}