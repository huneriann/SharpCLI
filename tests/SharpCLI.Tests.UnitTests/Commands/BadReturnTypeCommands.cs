namespace SharpCLI.Tests.UnitTests.Commands;

internal class BadReturnTypeCommands : ICommandsContainer
{
    [Command("bad-return")]
    public string BadReturn() => "invalid";
}