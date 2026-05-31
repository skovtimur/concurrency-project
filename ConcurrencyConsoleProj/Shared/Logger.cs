namespace ConcurrencyConsoleProj.Shared;

public static class Logger
{
    public static void Log(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        Console.WriteLine(message);
        Console.ForegroundColor = prevColor;
    }

    public static void LogWarn(string message)
    {
        Log(message, ConsoleColor.Yellow);
    }

    public static void LogError(string message)
    {
        Log(message, ConsoleColor.Red);
    }
}