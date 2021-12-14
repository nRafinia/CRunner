namespace CRunner.Models;

public class Address
{
    public string Include { get; set; }
    public Dictionary<string, IpConfig> IP { get; set; }
}