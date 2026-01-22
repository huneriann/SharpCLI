namespace SharpCLI.Tests.UnitTests.Commands;

internal abstract class StaticCommands : ICommandsContainer
{
    public static bool WasExecuted { get; private set; }

    [Command("static-test")]
    public static void StaticMethod()
    {
        WasExecuted = true;
    }
    
    [Command("throw-error", Description = "A command that always fails")]
    public static void ThrowError()
    {
        throw new InvalidOperationException("Standard exception occurred");
    }
}