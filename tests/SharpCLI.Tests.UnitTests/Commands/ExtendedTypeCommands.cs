namespace SharpCLI.Tests.UnitTests.Commands;

internal class ExtendedTypeCommands : ICommandsContainer
{
    public string? LastValue { get; private set; }

    [Command("extended-types")]
    public void ExtendedTypes(string s, float f, long l, decimal d)
    {
        LastValue = string.Create(CultureInfo.InvariantCulture, $"{s}|{f}|{l}|{d}");
    }

    [Command("fallback-types")]
    public void FallbackTypes(DateTime dt, int? nullableInt)
    {
        LastValue = string.Create(CultureInfo.InvariantCulture, $"{dt:d}|{nullableInt}");
    }
}