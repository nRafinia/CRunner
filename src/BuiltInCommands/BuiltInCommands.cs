namespace CRunner.BuiltInCommands;

public class BuiltInCommands : ICommandList
{
    public IDictionary<string, Type> CommandItems => new Dictionary<string, Type>()
    {
        {"sleep",typeof(Sleep)},
        {"print",typeof(Print)},
        {"println",typeof(Println)},
    };
}