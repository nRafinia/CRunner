namespace CRunner.Models;

public class IpConfig
{
    public IpConfig()
    {

    }

    public IpConfig(Security security, ConnectMode mode)
    {
        Security = security;
        Mode = mode;
    }

    public Security Security { get; set; }
    public ConnectMode Mode { get; set; }
}