using SharpCLI;

// Testing "Help" part
args = ["help"];

// Basic method
// args = ["basic-method"];
//
// // Method with arguments
// args = ["-mwa", "Mamed", "ALIYEV"];
//
// // Method with arguments and options
// args = ["-mwao", "Mamed", "ALIYEV", "-a", "26", "-vc", "10" /*not sending value, default value is 100*/];
//
// // Async method
// args = ["-asyncm", "Mamed"];

var app = new SharpCliHost("cli-app-static-methods", "CLI Application with Static methods")
    .RegisterCommands<StaticCommands>();

return await app.RunAsync(args);

class StaticCommands : ICommandsContainer
{
    [Command("basic-method", Aliases = ["-b"], Description = "Basic method. No return value, no parameter")]
    public static void BasicMethod()
    {
        Console.WriteLine("Basic Method");
    }

    [Command("method-with-arguments", Aliases = ["-mwa"], Description = "Method with arguments")]
    public static void MethodWithArguments(
        [Argument("firstName")] string firstName,
        [Argument("lastName")] string lastName)
    {
        Console.WriteLine($"MethodWithArguments: {firstName} {lastName}");
    }

    [Command("method-with-arguments-and-options", Aliases = ["-mwao"],
        Description = "Method with arguments and options")]
    public static void MethodWithArgumentsAndOptions(
        [Argument("firstName")] string firstName,
        [Argument("lastName")] string lastName,
        [Option("a", "age")] int age,
        [Option("vc", "visited-countries", DefaultValue = 100)]
        int visitedCountries)
    {
        Console.WriteLine(
            $"MethodWithArgumentsAndOptions: {firstName} {lastName} with age: {age}. Total visited countries: {visitedCountries}");
    }

    [Command("async-method", Aliases = ["-asyncm"], Description = "Async method")]
    public static Task AsyncMethod([Argument("firstName")] string firstName)
    {
        Console.WriteLine($"{firstName}");

        return Task.CompletedTask;
    }
}