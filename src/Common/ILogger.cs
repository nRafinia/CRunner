namespace CRunner;

public interface ILogger
{
    void WriteGreen(string text);
    void WriteLineGreen(string text);
    void WriteMagenta(string text);
    void WriteLineMagenta(string text);
    void WriteGray(string text);
    void WriteLineGray(string text);
    void WriteDarkRed(string text);
    void WriteLineDarkRed(string text);
    void Write(string text, ConsoleColor color);
    void Write(string text);
    void WriteLine(string text, ConsoleColor color);
    void WriteLine(string text);
    public void MarkupWrite(string text);
    public void MarkupWriteLine(string text);
    void ResetColor();

}