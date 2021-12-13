using Renci.SshNet;

namespace SSH.Providers.Ssh;

public class SshService
{
    private ConnectionInfo _connectionInfo;
    private string _ip;

    public void Connect(string ip, string username, string password)
    {
        _ip = ip;
        _connectionInfo = new ConnectionInfo(ip,
            22,
            username,
            new PasswordAuthenticationMethod(username, password)
            /*new PrivateKeyAuthenticationMethod("rsa.key")*/);
    }

    public async Task Run(string[] secs, string[] commands)
    {
        using var client = new SshClient(_connectionInfo);

        client.Connect();

        if (client.IsConnected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Connected to {_ip}");
            Console.WriteLine("");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Could not connect, disconnected.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        var stream = client.CreateShellStream("Customcommand", 0, 0, 0, 0, 10);//1024);
                                                                               //var reader = new StreamReader(stream);

        stream.DataReceived += (sender, eventArgs) =>
        {
            var data = System.Text.Encoding.UTF8.GetString(eventArgs.Data);
            Console.Write(data);
        };

        await Task.Delay(1000);

        for (var i = 2; i < secs.Length; i++)
        {
            var cmd = secs[i];

            stream.WriteLine(cmd);
            await stream.FlushAsync();
        }

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

        client.Disconnect();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        Console.WriteLine($"Disconnected from {_ip}");

        await Task.Delay(1000);

    }
}