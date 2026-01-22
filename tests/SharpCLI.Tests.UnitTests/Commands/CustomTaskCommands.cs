namespace SharpCLI.Tests.UnitTests.Commands;

public class CustomTaskCommands : ICommandsContainer
{
    [Command("custom-task", Description = "Returns a Task-derived type to hit default case")]
    public Task CustomTaskCommand()
    {
        return Task.CompletedTask;
    }
}