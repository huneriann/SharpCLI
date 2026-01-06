namespace SharpCLI.Tests.UnitTests.Commands;

internal class DuplicateParamNamesCommands : ICommandsContainer
{
    [Command("dup-params")]
    public void DupParams([Argument("same")] string param1, [Argument("same")] int param2)
    {
    }
}