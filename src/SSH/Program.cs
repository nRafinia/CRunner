using Renci.SshNet;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.CursorVisible = false;

Console.WriteLine("Starting...");
Console.WriteLine("");

var ips = File.ReadAllLines("ip.txt");
var commands = File.ReadAllLines("command.txt");
var sec = File.ReadAllLines("sec.txt");

foreach (var ip in ips)
{
    var connectionInfo = new ConnectionInfo(ip,
                                            22,
                                            sec[0],
                                            new PasswordAuthenticationMethod(sec[0], sec[1])
                                            /*new PrivateKeyAuthenticationMethod("rsa.key")*/);

    try
    {

        using var client = new SshClient(connectionInfo);

        client.Connect();

        if (client.IsConnected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Connected to {ip}");
            Console.WriteLine("");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Could not connect, disconnected.");
            break;
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

        for (var i = 2; i < sec.Length; i++)
        {
            var cmd = sec[i];

            stream.WriteLine(cmd);
            await stream.FlushAsync();
        }

        foreach (var cmd in commands)
        {
            if (cmd.StartsWith("sleep"))
            {
                var sleep = cmd.Split(":");
                await Task.Delay(int.Parse(sleep[1]));
                continue;
            }

            stream.WriteLine(cmd);
            await stream.FlushAsync();

        }

        client.Disconnect();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        Console.WriteLine($"Disconnected from {ip}");

        await Task.Delay(1000);
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