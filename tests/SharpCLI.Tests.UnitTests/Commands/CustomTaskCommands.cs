namespace SharpCLI.Tests.UnitTests.Commands;

public class CustomTaskCommands : ICommandsContainer
{
    [Command("custom-task", Description = "Returns a Task-derived type to hit default case")]
    public Task CustomTaskCommand()
    {
        // Returning a completed task but structured to fall through 
        // logic that specifically checks for Task<int> or Task
        return Task.CompletedTask; 
    }
}