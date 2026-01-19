namespace SharpCLI.Tests.UnitTests.Commands;

public class FallbackTypeCommands : ICommandsContainer
{
    public byte LastByteValue { get; private set; }

    [Command("byte-test", Description = "Tests fallback to Convert.ChangeType")]
    public void ByteCommand([Argument("value")] byte value)
    {
        LastByteValue = value;
    }
}