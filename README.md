# SharpCLI

![sharp-cli-icon](https://github.com/huneriann/sharpcli/blob/master/icon.png?raw=true)

[![NuGet](https://img.shields.io/nuget/v/sharpcli.svg)](https://www.nuget.org/packages/sharpcli)
[![publish to nuget](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml/badge.svg)](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml)
---

> **A modern, attribute-based CLI framework for .NET** 

Transform your methods into powerful CLI commands using simple attributes with automatic arguments parsing, help
generation, and async support.

## Acknowledgments

SharpCLI's design and architecture are derived from the foundational concepts established by [Python Click](https://github.com/pallets/click), developed by the Pallets team. We acknowledge their significant contribution to the CLI development paradigm and extend our appreciation for their innovative approach to command-line interface design.

##  Features

- **Method-Based Commands** - Turn any static method into a CLI command with a simple attribute
- **Attribute-Driven** - Use `[Command]`, `[Argument]`, and `[Option]` attributes to define your CLI
- **Automatic Parsing** - Built-in argument parsing with type conversion and validation
- **Auto-Generated Help** - Beautiful help text generated from your attributes
- **Async Support** - Full support for async commands with `Task<int>` and `Task`
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
    var app = new SharpCliHost("cli-app", "An awesome CLI application")
        .RegisterCommands<Commands>();
        
    return await app.RunAsync(args);
    
    public class Commands
    {
        [Command("hello-world", Description = "Hello World!")]
        public static int HelloWorld() 
        {
            Console.WriteLine("SharpCLI is awesome!"); 
            return 0;
        }
    
        [Command("greet", Description = "Greet someone", Aliases = ["hello", "hi"])]
        public static int Greet(
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
    # Greet someone
    cli-app greet Alex
    
    # Use options
    cli-app greet Bob --message "Hi there" --count 3
    
    # Use short options
    cli-app hello Charlie -m "Hey" -c 2
    
    # Get help
    cli-app --help
    cli-app greet --help
```

## Advanced Features

### Async Commands
```csharp
    [Command("download")]
    public static async Task<int> Download(
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
    public static int Deploy(
        [Argument("environment")] Environment env, // Enum support
        [Option("t", "timeout")] TimeSpan timeout = default) // Custom types
    {
        // Implementation
    }
```
##  License
This project is licensed under the MIT License - see the [MIT License](https://github.com/huneriann/sharpcli/blob/master/LICENSE.md) file for details.
