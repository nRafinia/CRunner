using CRunner.Models;

namespace CRunner.Providers;

public interface IProvider : IDisposable
{
    bool Connect(string ip, Security security);
    void Disconnect();
    Task Run(IEnumerable<string> commands);
}