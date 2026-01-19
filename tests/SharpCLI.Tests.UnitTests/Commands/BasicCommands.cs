namespace SharpCLI.Tests.UnitTests.Commands;

using System.Globalization;

internal class BasicCommands : ICommandsContainer
{
    public bool WasExecuted { get; private set; }
    public string? LastValue { get; private set; }
    public int LastInt { get; private set; }
    public double LastDouble { get; private set; }

    [Command("test", Description = "A test command", Aliases = ["t"])]
    public int ExecuteTest(
        [Argument("input", Description = "Test input")]
        string input,
        [Option("c", "count", Description = "A count")]
        int count)
    {
        WasExecuted = true;
        LastValue = $"{input}-{count}";
        return 0;
    }

    [Command("math", Description = "Math command for negative numbers")]
    public int MathCommand(
        int val, 
        [Option("o", "offset")] double offset = 0)
    {
        WasExecuted = true;
        LastInt = val;
        LastDouble = offset;
        return 0;
    }

    [Command("async-test")]
    public async Task<int> ExecuteAsyncTest()
    {
        await Task.Yield();
        WasExecuted = true;
        return 42;
    }

    [Command("bool-test")]
    public int ExecuteBool([Option("f", "force")] bool force)
    {
        WasExecuted = true;
        return force ? 1 : 0;
    }

    [Command("types")]
    public int TypeTest(int i, double d, bool b, TestLevel level)
    {
        WasExecuted = true;
        LastValue = string.Create(CultureInfo.InvariantCulture, $"{i}|{d}|{b}|{level}");
        return 0;
    }

    [Command("multi-arg")]
    public int MultiArg(string first, string second)
    {
        WasExecuted = true;
        LastValue = $"{first}-{second}";
        return 0;
    }
    
    [Command("throw-error", Description = "A command that always fails")]
    public void ThrowError()
    {
        throw new InvalidOperationException("Standard exception occurred");
    }
}