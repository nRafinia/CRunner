using CRunner;
using CRunner.Models;
using CRunner.Providers;
using Microsoft.Extensions.DependencyInjection;

var setting = await LoadConfig();

if (setting is null)
{
    return;
}

var serviceProvider = new ServiceCollection()
    .AddSingleton<CommandService>()
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
    var settingPath = args.Any()
        ? args[0]
        : "setting.yml";

    return await SettingLoader.Load(settingPath);
}