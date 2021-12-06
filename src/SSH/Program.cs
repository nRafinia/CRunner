using Renci.SshNet;

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
        var stream = client.CreateShellStream("Customcommand", 0, 0, 0, 0, 1024);
        var reader = new StreamReader(stream);

        for (var i = 2; i < sec.Length; i++)
        {
            var cmd = sec[i];

            stream.WriteLine(cmd);
            stream.Flush();
        }

        foreach (var cmd in commands)
        {
            if (cmd.StartsWith("sleep"))
            {
                var sleep = cmd.Split(":");
                Thread.Sleep(int.Parse(sleep[1]));
                continue;
            }

            stream.WriteLine(cmd);
            stream.Flush();
            Console.WriteLine(reader.ReadToEnd());
        }

        Console.WriteLine(reader.ReadToEnd());

        client.Disconnect();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Disconnected from {ip}");
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"Error in connect to {ip}. Error={e.Message}");
    }

    Console.WriteLine("");
}

Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("Finished.");

Console.WriteLine("");
Console.WriteLine("Press Enter to exit...");
Console.ReadLine();
