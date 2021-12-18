namespace CRunner.BuiltInCommands;

public class Sleep : ICommand
{
    public string CommandName => "sleep";

    public async Task GetCommand(ICommandRunner commandRunner, IEnumerable<string> parameters, ILogger logger)
    {
        if (parameters is null || !parameters.Any())
        {
            return;
        }

        var delay = parameters.ElementAt(0);

        if (!int.TryParse(delay, out var delayValue))
        {
            return;
        }

        logger.WriteLineGreen("");
        logger.WriteLineGreen($" --> Sleep for {delayValue} millisecond...");

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(delayValue));
        await periodicTimer.WaitForNextTickAsync();
        periodicTimer.Dispose();
    }
}