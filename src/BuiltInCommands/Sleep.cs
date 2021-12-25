namespace CRunner.BuiltInCommands;

public class Sleep : ICommand
{
    public async Task RunCommand(IEnumerable<string> parameters)
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

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(delayValue));
        await periodicTimer.WaitForNextTickAsync();
        periodicTimer.Dispose();
    }
}