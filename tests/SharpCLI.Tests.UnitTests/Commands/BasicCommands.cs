namespace SharpCLI.Tests.UnitTests.Commands;

internal class BasicCommands : ICommandsContainer
{
    public bool WasExecuted { get; private set; }
    public string? LastValue { get; private set; }


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

    [Command("async-test")]
    public async Task<int> ExecuteAsyncTest()
    {
        await Task.Yield();
        return 42;
    }

    [Command("bool-test")]
    public int ExecuteBool([Option("f", "force")] bool force)
    {
        return force ? 1 : 0;
    }

    [Command("types")]
    public int TypeTest(int i, double d, bool b, TestLevel level)
    {
        LastValue = string.Create(CultureInfo.InvariantCulture, $"{i}|{d}|{b}|{level}");
        return 0;
    }

    [Command("multi-arg")]
    public int MultiArg(string first, string second)
    {
        LastValue = $"{first}-{second}";
        return 0;
    }
}