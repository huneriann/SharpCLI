namespace SharpCLI.Tests.UnitTests.Commands;

public class UndocumentedCommands : ICommandsContainer
{
    [Command("documented-cmd", Description = "Has a description")]
    public void Documented()
    {
    }

    [Command("undocumented-cmd")]
    public void Undocumented()
    {
    }
}