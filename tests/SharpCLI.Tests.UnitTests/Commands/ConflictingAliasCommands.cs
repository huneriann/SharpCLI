namespace SharpCLI.Tests.UnitTests.Commands;

internal class ConflictingAliasCommands : ICommandsContainer
{
    [Command("other", Aliases = new[] { "t" })]
    public void OtherCommand()
    {
    }
}