namespace CRunner.BuiltInCommands;

public class Print : ICommand
{
    private readonly ILogger _logger;
    public Print(ILogger logger)
    {
        _logger = logger;
    }

    public Task RunCommand(IEnumerable<string> parameters)
    {
        if (parameters is null || !parameters.Any())
        {
            _logger.Write("");
            return Task.CompletedTask;
        }

        _logger.Write(string.Join(' ', parameters));

        return Task.CompletedTask;
    }
}