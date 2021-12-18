using Renci.SshNet;

namespace CRunner.Providers;

public class SshCommandRunner : ICommandRunner
{
    private readonly ShellStream _stream;
    public SshCommandRunner(ShellStream stream)
    {
        _stream = stream;
    }

    public Task Run(string command)
    {
        _stream.WriteLine(command);
        return _stream.FlushAsync();
    }
}