using CRunner;
using CRunner.Models;
using CRunner.Providers;
using Microsoft.Extensions.DependencyInjection;

var commandService = new CommandService();
await commandService.LoadCommands();

var setting = await LoadConfig();

if (setting is null)
{
    return;
}

setting = commandService.ProcessConfig(setting);


var serviceCollection = new ServiceCollection();

var moduleService = new ModuleService(null);
moduleService.LoadCommandModules(serviceCollection);

var serviceProvider = serviceCollection
    .AddSingleton<ModuleService>()
    .AddSingleton<Startup>()
    .AddSingleton<SshService>()
    .AddSingleton<TelnetService>()
    .AddSingleton(setting)
    .AddSingleton<ILogger>(new Logger())
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