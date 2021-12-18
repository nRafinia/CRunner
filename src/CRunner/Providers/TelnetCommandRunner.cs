using System.Net.Sockets;

namespace CRunner.Providers;

public class TelnetCommandRunner : ICommandRunner
{
    private readonly NetworkStream _stream;

    public TelnetCommandRunner(NetworkStream stream)
    {
        _stream = stream;
    }

    public async Task Run(string command)
    {
        var cmdByte = System.Text.Encoding.ASCII.GetBytes(command);
        await _stream.WriteAsync(cmdByte, 0, cmdByte.Length);
        _stream.WriteByte(13);
        await _stream.FlushAsync();
    }
}