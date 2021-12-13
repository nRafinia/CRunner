namespace CRunner.Providers;

public class Logger
{
    #region Green
    public void WriteGreen(string text)
    {
        Write(text, ConsoleColor.Green);
    }

    public void WriteLineGreen(string text)
    {
        WriteGreen(text + Environment.NewLine);
    }
    #endregion

    #region Magenta
    public void WriteMagenta(string text)
    {
        Write(text, ConsoleColor.Magenta);
    }

    public void WriteLineMagenta(string text)
    {
        WriteMagenta(text + Environment.NewLine);
    }
    #endregion

    #region Gray
    public void WriteGray(string text)
    {
        Write(text, ConsoleColor.Gray);
    }

    public void WriteLineGray(string text)
    {
        WriteGray(text + Environment.NewLine);
    }
    #endregion

    #region Gray
    public void WriteDarkRed(string text)
    {
        Write(text, ConsoleColor.DarkRed);
    }

    public void WriteLineDarkRed(string text)
    {
        WriteDarkRed(text + Environment.NewLine);
    }
    #endregion

    #region Write

    public void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Write(text);
    }

    public void WriteLine(string text, ConsoleColor color)
    {
        Write(text + Environment.NewLine, color);
    }

    public void WriteLine(string text)
    {
        Write(text + Environment.NewLine);
    }

    public void Write(string text)
    {
        Console.Write(text);
    }

    #endregion
}