namespace SharpCLI.Benchmarks.Commands;

using SharpCLI;

public class SampleStaticCommands : ICommandsContainer
{
    [Command("hello", Description = "A simple hello command", Aliases = new[] { "hi" })]
    public static void HelloCommand(string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }

    [Command("add", Description = "Adds two numbers")]
    public static int AddCommand(int a, int b)
    {
        return a + b;
    }

    [Command("async-task", Description = "Async command")]
    public static async Task AsyncCommand(string message)
    {
        await Task.Delay(1); // Simulate async work
        Console.WriteLine(message);
    }

    [Command("options-test", Description = "Command with options")]
    public static void OptionsCommand(
        [Option("v", "verbose", Description = "Enable verbose output")] bool verbose = false,
        [Option("c", "count", Description = "Count value")] int count = 0)
    {
        if (verbose) Console.WriteLine($"Count: {count}");
    }

    [Command("mixed", Description = "Mixed arguments and options")]
    public static void MixedCommand(
        string arg1,
        [Argument("arg2", Required = false)] int arg2 = 0,
        [Option("f", "flag")] bool flag = false,
        [Option("v", "value", Description = "A string value")] string value = "default")
    {
        Console.WriteLine($"{arg1} {arg2} {flag} {value}");
    }

    [Command("enum-test", Description = "Command with enum")]
    public static void EnumCommand(ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine("Colored text");
        Console.ResetColor();
    }

    [Command("many-args", Description = "Command with many positional args")]
    public static void ManyArgsCommand(string arg1, string arg2, string arg3, string arg4, string arg5)
    {
        Console.WriteLine($"{arg1} {arg2} {arg3} {arg4} {arg5}");
    }
}