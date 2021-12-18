namespace CRunner;

public interface ICommandRunner
{
    Task Run(string command);
}