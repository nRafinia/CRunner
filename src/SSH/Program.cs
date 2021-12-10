using Microsoft.Extensions.DependencyInjection;
using SSH;
using SSH.Providers.Ssh;

var serviceProvider = new ServiceCollection()
    .AddSingleton<Startup>()
    .AddSingleton<SshService>()
    .BuildServiceProvider();

var startup = serviceProvider.GetService<Startup>();

await startup.Start();

