namespace SharpCLI.Tests.UnitTests.Commands;

internal class MixedCommands : ICommandsContainer
{
    [Command("static-cmd")]
    public static void StaticMethod()
    {
    }

    [Command("instance-test")]
    public void InstanceMethod()
    {
    }
}