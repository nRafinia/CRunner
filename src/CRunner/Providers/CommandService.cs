using CRunner.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CRunner.Providers;

public class CommandService
{
    private readonly IDictionary<string, BatchCommand> _batchCommands = new Dictionary<string, BatchCommand>();

    public async Task LoadCommands()
    {
        if (!Directory.Exists("commands"))
        {
            Directory.CreateDirectory("commands");
        }

        var commandFiles = Directory.GetFiles("commands", "*.yml");
        foreach (var commandFile in commandFiles)
        {
            var batchCommand = await LoadConfig(commandFile);

            if (batchCommand is null || string.IsNullOrWhiteSpace(batchCommand.CommandName) ||
                batchCommand.Commands is null || !batchCommand.Commands.Any())
            {
                continue;
            }

            if (_batchCommands.ContainsKey(batchCommand.CommandName))
            {
                Console.WriteLine($"Command files have duplicate CommandName with '{batchCommand.CommandName}'. duplicated file is '{commandFile}'");
                Environment.Exit(0);
                return;
            }

            _batchCommands.Add(batchCommand.CommandName, batchCommand);
        }
    }

    public RunSetting ProcessConfig(RunSetting setting)
    {
        var commands = new List<string>();
        foreach (var command in setting.Commands.Lines)
        {
            if (!_batchCommands.ContainsKey(command))
            {
                commands.Add(command);
                continue;
            }

            var batchCommand = _batchCommands[command];
            commands.AddRange(batchCommand.Commands);
        }

        setting.Commands.Lines = commands;

        return setting;
    }

    private static async Task<BatchCommand> LoadConfig(string path)
    {
        var batchFile = await File.ReadAllTextAsync(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<BatchCommand>(batchFile);
    }
}