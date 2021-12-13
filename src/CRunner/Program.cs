using CRunner;
using CRunner.Models;
using CRunner.Providers;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var setting = await LoadConfig();
var serviceProvider = new ServiceCollection()
    .AddSingleton<Startup>()
    .AddSingleton<SshService>()
    .AddSingleton<TelnetService>()
    .AddSingleton(setting)
    .AddSingleton(new Logger())
    .BuildServiceProvider();

var startup = serviceProvider.GetService<Startup>();

await startup.Start();

async Task<RunSetting> LoadConfig()
{
    var settingName = args.Any()
        ? args[0]
        : "setting.yml";

    var settingFile = await File.ReadAllTextAsync(settingName);

    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(NullNamingConvention.Instance)  // see height_in_inches in sample yml 
        .Build();

    var setting = deserializer.Deserialize<RunSetting>(settingFile);
    return setting;
}