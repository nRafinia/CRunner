using Microsoft.Extensions.DependencyInjection;
using SSH;
using SSH.Providers.Telnet;


var serviceProvider = new ServiceCollection()
    .AddSingleton<Startup>()
    //.AddSingleton<SshService>()
    .AddSingleton<TelnetService>()
    .BuildServiceProvider();

var startup = serviceProvider.GetService<Startup>();

await startup.Start();

