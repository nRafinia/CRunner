namespace CRunner.Models;

public class BatchCommand
{
    public string CommandName { get; set; }
    public IEnumerable<string> Commands { get; set; }
}