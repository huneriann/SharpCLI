namespace SharpCLI.Benchmarks.Commands;

using SharpCLI;

public class SampleInstanceCommands : ICommandsContainer
{
    private int _state;

    [Command("increment", Description = "Increments state")]
    public void Increment()
    {
        _state++;
        Console.WriteLine(_state);
    }
}