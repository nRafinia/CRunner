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

        var ips = _setting.Address.IP;
        foreach (var ip in ips)
        {
            try
            {
                var security = ip.Value == null
                    ? _setting.Security
                    : ip.Value;

                var ssh = _provider.GetService<SshService>();
                ssh.Connect(ip.Key, security);
                await ssh.Run(_setting.Commands.Lines);

                /*var telnet = _provider.GetService<TelnetService>();
                telnet.Connect(ip.Key, security);
                await telnet.Run(_setting.Commands.Lines);*/
            }
            catch (Exception e)
            {
                _logger.WriteLine("");
                _logger.WriteLineDarkRed($"Error in connect to {ip}. Error={e.Message}");
            }

            await Task.Delay(1000);
            _logger.WriteLine("");
        }

        _logger.WriteLineGray("Finished.");

        _logger.WriteLineGray("");
        _logger.WriteGray("Press Enter to exit....");
        Console.ReadLine();
    }

}