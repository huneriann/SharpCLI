using SharpCLI;

var host = new SharpCliHost("MinimalApiCli", "test cli on Minimal API");

host.RegisterCommands<Commands>();

await host.RunAsync(["test"]);

public class Commands
{
    [Command("test")]
    public static int Test()
    {
        Console.WriteLine("Test");
        return 0;
    }
}