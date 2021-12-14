using CRunner.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CRunner.Providers;

public static class SettingLoader
{
    public static async Task<RunSetting> Load(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Setting file '{path}' not found!");
            Environment.Exit(0);
            return default;
        }

        var settingDirectory = Path.GetDirectoryName(path);
        var setting = await LoadConfig(path);
        var res = await ProcessConfig(setting, settingDirectory);

        return res;
    }

    private static async Task<RunSetting> LoadConfig(string path)
    {
        var settingFile = await File.ReadAllTextAsync(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<RunSetting>(settingFile);
    }

    private static async Task<RunSetting> ProcessConfig(RunSetting setting, string path)
    {
        var res = new RunSetting
        {
            Address = await CheckAddress(setting.Address, path),
            Security = await CheckSecurity(setting.Security, path),
            Commands = await CheckCommands(setting.Commands, path),
        };

        return res;
    }

    private static async Task<Address> CheckAddress(Address address, string path)
    {
        var res = new Address()
        {
            IP = address.IP ?? new Dictionary<string, IpConfig>()
        };

        if (string.IsNullOrWhiteSpace(address.Include))
        {
            return res;
        }

        var includedFilePath = Path.Combine(path, address.Include);
        var setting = await LoadConfig(includedFilePath);

        if (setting?.Address?.IP == null)
        {
            return res;
        }

        foreach (var ipConfig in setting.Address.IP)
        {
            if (res.IP.ContainsKey(ipConfig.Key))
            {
                continue;
            }
            res.IP.Add(ipConfig.Key, ipConfig.Value);
        }

        return res;
    }

    private static async Task<Security> CheckSecurity(Security security, string path)
    {
        var res = new Security()
        {
            UserName = security.UserName,
            Password = security.Password
        };

        if (string.IsNullOrWhiteSpace(security.Include))
        {
            return res;
        }

        var includedFilePath = Path.Combine(path, security.Include);
        var setting = await LoadConfig(includedFilePath);

        if (setting?.Security == null)
        {
            return res;
        }

        if (!string.IsNullOrWhiteSpace(setting.Security.UserName))
        {
            res.UserName = setting.Security.UserName;
        }
        if (!string.IsNullOrWhiteSpace(setting.Security.Password))
        {
            res.Password = setting.Security.Password;
        }

        return res;
    }

    private static async Task<Commands> CheckCommands(Commands commands, string path)
    {
        if (string.IsNullOrWhiteSpace(commands.Include))
        {
            return commands;
        }

        var includedFilePath = Path.Combine(path, commands.Include);
        var setting = await LoadConfig(includedFilePath);

        if (setting?.Commands?.Lines == null)
        {
            return commands;
        }

        return new Commands()
        {
            Lines = setting.Commands.Lines.Concat(commands.Lines)
        };
    }

}