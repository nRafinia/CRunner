using CRunner.Models;

namespace CRunner.Providers;

public interface IProvider : IDisposable
{
    void Connect(string ip, Security security);
    Task Run(IEnumerable<string> commands);
}