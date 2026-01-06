namespace SharpCLI.Tests.UnitTests.Commands;

internal class HelpTestCommands : ICommandsContainer
{
    [Command("z-cmd", Description = "Zebra description")]
    public void Zebra()
    {
    }

    [Command("a-cmd", Description = "Alpha description")]
    public void Alpha()
    {
    }
}