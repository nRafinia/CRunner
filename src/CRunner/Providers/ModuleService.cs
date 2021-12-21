using System.Reflection;

namespace CRunner.Providers;

public class ModuleService
{
    private readonly List<ICommand> _commandClass = new();
    private readonly List<Assembly> _assemblies = new();

    public ModuleService()
    {
        LoadCommandModules();
    }

    public bool Exist(string commandName)
    {
        return _commandClass.Any(_ =>
            string.Equals(_.CommandName, commandName, StringComparison.CurrentCultureIgnoreCase));
    }

    public ICommand Get(string commandName)
    {
        return _commandClass.FirstOrDefault(_ =>
            string.Equals(_.CommandName, commandName, StringComparison.CurrentCultureIgnoreCase));
    }

    private void LoadCommandModules()
    {
        LoadCommandLibraries();
        LoadCommandClass();
    }

    private void LoadCommandLibraries()
    {
        var modules = Directory.GetFiles("modules", "*.dll");
        var currentDir = Directory.GetCurrentDirectory();
        foreach (var module in modules)
        {
            var fileByte = File.ReadAllBytes(Path.Combine(currentDir, module));
            var asm = Assembly.Load(fileByte);
            _assemblies.Add(asm);
        }
    }

    private void LoadCommandClass()
    {
        //var classes = Globals.GetImplementedInterfaceOf<ICommand>();
        foreach (var assembly in _assemblies)
        {
            var classes = assembly.GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var item in classes)
            {
                var command = Activator.CreateInstance(item) as ICommand;
                _commandClass.Add(command);
            }
        }
    }
}