namespace SharpCLI.Tests.UnitTests.Commands;

public class AsyncVoidTaskCommands : ICommandsContainer
{
    public bool WasExecuted { get; private set; }

    [Command("void-task", Description = "Returns a plain Task")]
    public async Task VoidTaskCommand()
    {
        await Task.Yield();
        WasExecuted = true;
    }
}