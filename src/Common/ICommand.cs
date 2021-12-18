namespace CRunner;

public interface ICommand
{
    string CommandName { get; }

    Task GetCommand(ICommandRunner commandRunner, IEnumerable<string> parameters);
}
