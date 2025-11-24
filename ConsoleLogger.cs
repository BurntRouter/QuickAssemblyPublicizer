namespace QuickAssemblyPublicizer;

public sealed class ConsoleLogger : ILogger
{
    public void Info(string message)
    {
        WriteLine("INFO", message);
    }

    public void Warn(string message)
    {
        WriteLine("WARN", message);
    }

    public void Error(string message)
    {
        WriteLine("ERROR", message);
    }

    private static void WriteLine(string level, string message)
    {
        Console.WriteLine($"[{level}] {message}");
    }
}

