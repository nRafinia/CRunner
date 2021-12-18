using CRunner.Tools;

namespace CRunner.Providers;

public class CommandService
{
    private readonly List<ICommand> _commandClass = new();

    public CommandService()
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

    }

    private void LoadCommandClass()
    {
        var classes = Globals.GetImplementedInterfaceOf<ICommand>();
        foreach (var item in classes)
        {
            var command = Activator.CreateInstance(item) as ICommand;
            _commandClass.Add(command);
        }
    }
}