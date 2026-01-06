namespace SharpCLI.Tests.UnitTests.Commands;

internal class StaticCommands : ICommandsContainer
{
    public static bool WasExecuted { get; private set; }

    [Command("static-test")]
    public static void StaticMethod()
    {
        WasExecuted = true;
    }
}