using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CRunner.Providers;

public class ModuleService
{
    private static readonly IDictionary<string, Type> CommandClass = new Dictionary<string, Type>();

    private readonly IServiceProvider _provider;
    public ModuleService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void LoadCommandModules(IServiceCollection serviceCollection)
    {
        var assemblies = LoadCommandLibraries();
        LoadCommandClass(assemblies, serviceCollection);
    }

    public bool Exist(string commandName)
    {
        return CommandClass.ContainsKey(commandName);
    }

    public ICommand Get(string commandName)
    {
        if (!Exist(commandName))
        {
            return default;
        }

        var command = CommandClass[commandName];
        var commandProvider = _provider.GetService(command) as ICommand;
        return commandProvider;
    }

    private IEnumerable<Assembly> LoadCommandLibraries()
    {
        var assemblies = new List<Assembly>();

        if (!Directory.Exists("modules"))
        {
            Directory.CreateDirectory("modules");
        }

        var modules = Directory.GetFiles("modules", "*.dll");
        var currentDir = Directory.GetCurrentDirectory();
        foreach (var module in modules)
        {
            var fileByte = File.ReadAllBytes(Path.Combine(currentDir, module));
            var asm = Assembly.Load(fileByte);
            assemblies.Add(asm);
        }

        return assemblies;
    }

    private void LoadCommandClass(IEnumerable<Assembly> assemblies, IServiceCollection serviceCollection)
    {
        foreach (var assembly in assemblies)
        {
            var commandLists = assembly.GetTypes()
                .Where(t => typeof(ICommandList).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var commandList in commandLists)
            {
                var commandListClass = Activator.CreateInstance(commandList) as ICommandList;

                foreach (var command in commandListClass?.CommandItems ?? new Dictionary<string, Type>())
                {
                    //if (!(command.Value is ICommand))
                    if (!typeof(ICommand).IsAssignableFrom(command.Value))
                    {
                        continue;
                    }

                    CommandClass.Add(command);
                    serviceCollection.AddTransient(command.Value);
                }
            }
        }
    }
}