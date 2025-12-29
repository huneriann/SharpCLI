# SharpCLI

---

![sharp-cli-icon](https://raw.githubusercontent.com/huneriann/sharpcli/master/icon.png?raw=true)

[![NuGet](https://img.shields.io/nuget/v/sharpcli.svg)](https://www.nuget.org/packages/sharpcli)
[![Nuget Downloads](https://img.shields.io/nuget/dt/SharpCLI)](https://www.nuget.org/packages/sharpcli)
[![publish to nuget](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml/badge.svg)](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml)

> **A modern, attribute-based CLI framework for .NET**

Transform your methods into powerful CLI commands using simple attributes with automatic arguments parsing, help
generation, and async support.

## Acknowledgments

SharpCLI's design and architecture are derived from the foundational concepts established by [Python Click](https://github.com/pallets/click), developed by the Pallets team.

##  Features

- **Method-Based Commands** - Turn any method into a CLI command with a simple attribute
- **Attribute-Driven** - Use `[Command]`, `[Argument]`, and `[Option]` attributes to define your CLI
- **Automatic Parsing** - Built-in argument parsing with type conversion and validation
- **Auto-Generated Help** - Beautiful help text generated from your attributes
- **Async Support** - Full support for async commands with `Task`
- **Command Aliases** - Multiple ways to invoke the same command
- **Type Safety** - Strongly-typed parameters with compile-time checking
- **Zero Dependencies** - Lightweight with no external dependencies

## Installation
```text
dotnet add package SharpCli
```

## Quick Start

### 1. Define Your Commands, Register and Run

```csharp
// For static methods
var app = new SharpCliHost("cli-app", "An awesome CLI application")
    .RegisterCommands<StaticCommands>();
    
return await app.RunAsync(args);

public class StaticCommands : ICommandsContainer
{
    [Command("hello-world", Description = "Hello World!")]
    public static int HelloWorld() 
    {
        Console.WriteLine("SharpCLI is awesome!"); 
        return 0;
    }
}

// Or

// For non-static methods
var app = new SharpCliHost("cli-app", "An awesome CLI application")
    .RegisterCommands(new NonStaticCommands());

return await app.RunAsync(args);

public class NonStaticCommands : ICommandsContainer
{
    [Command("greet", Description = "Greet someone", Aliases = ["hello", "hi"])]
    public int Greet(
        [Argument("name", Description = "Person to greet")] string name,
        [Option("m", "message", Description = "Custom message")] string message = "Hello",
        [Option("c", "count", Description = "Times to greet")] int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{message}, {name}!");
        }
        return 0;
    }
}
```

### 2. Use Your CLI
```
# Basic usage
cli-app hello-world

# Usage with arguments and options
cli-app greet Bob -m "Hi there" --count 3

# Usage of help
cli-app --help
cli-app greet --help
```

## Advanced Features

### Async Commands
```csharp
[Command("download")]
public async Task<int> Download(
    [Argument("url")] string url,
    [Option("o", "output")] string output = "download")
{
    // Async implementation
    await DownloadFileAsync(url, output);
    return 0;
}
```

### Complex Types
```csharp
[Command("deploy")]
public int Deploy(
    [Argument("environment")] Environment env, // Enum support
    [Option("t", "timeout")] TimeSpan timeout = default) // Custom types
{
    // Implementation
}
```

##  License
This project is licensed under the MIT License - see the [MIT License](https://github.com/huneriann/sharpcli/blob/master/LICENSE.md) file for details.
