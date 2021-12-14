namespace CRunner.Models;

public class Commands
{
    public string Include { get; set; }
    public IEnumerable<string> Lines { get; set; }
}