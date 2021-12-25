namespace CRunner.BuiltInCommands;

public class Println : ICommand
{
    private readonly ILogger _logger;
    public Println(ILogger logger)
    {
        _logger = logger;
    }

    public Task RunCommand(IEnumerable<string> parameters)
    {
        if (parameters is null || !parameters.Any())
        {
            _logger.WriteLine("");
            return Task.CompletedTask;
        }

        _logger.WriteLine(string.Join(' ', parameters));

        return Task.CompletedTask;
    }
}