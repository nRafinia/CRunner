using Microsoft.Extensions.DependencyInjection;
using SSH.Providers.Ssh;

namespace SSH;

public class Startup
{
    private string[] ips;
    private string[] commands;
    private string[] sec;

    private readonly IServiceProvider _provider;
    public Startup(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        Console.WriteLine("Starting...");
        Console.WriteLine("");

        await LoadFiles();

        foreach (var ip in ips)
        {
            try
            {
                var ssh = _provider.GetService<SshService>();
                ssh.Connect(ip, sec[0], sec[1]);
                await ssh.Run(sec, commands);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("");
                Console.WriteLine($"Error in connect to {ip}. Error={e.Message}");
            }

            Console.WriteLine("");
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Finished.");
    }

    private async Task LoadFiles()
    {
        ips = await File.ReadAllLinesAsync("ip.txt");
        commands = await File.ReadAllLinesAsync("command.txt");
        sec = await File.ReadAllLinesAsync("sec.txt");

    }
}