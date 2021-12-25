namespace CRunner;

public interface ICommand
{
    Task RunCommand(IEnumerable<string> parameters);
}
