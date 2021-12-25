namespace CRunner;

public interface ICommandList
{
    IDictionary<string, Type> CommandItems { get; }
}