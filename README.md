# SharpCLI

![sharp-cli-icon](https://raw.githubusercontent.com/huneriann/sharpcli/master/icon.png?raw=true)

[![NuGet](https://img.shields.io/nuget/v/sharpcli.svg)](https://www.nuget.org/packages/sharpcli)
[![Nuget Downloads](https://img.shields.io/nuget/dt/SharpCLI)](https://www.nuget.org/packages/sharpcli)
[![publish to nuget](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml/badge.svg)](https://github.com/huneriann/sharpcli/actions/workflows/publish.yml)

> **A modern, attribute-based CLI framework for .NET**

Transform your methods into powerful CLI commands using simple attributes with automatic arguments parsing, help
generation, and async support.

## Acknowledgments

SharpCLI's design and architecture are derived from the foundational concepts established
by [Python Click](https://github.com/pallets/click), developed by the Pallets team.

## Features

- **Method-Based Commands** - Turn any method into a CLI command with a simple attribute
- **Attribute-Driven** - Use `[Command]`, `[Argument]`, and `[Option]` attributes to define your CLI
- **Automatic Parsing** - Built-in argument parsing with type conversion and validation
- **Auto-Generated Help** - Beautiful help text generated from your attributes
- **Async Support** - Full support for async commands with `Task`
- **Command Aliases** - Multiple ways to invoke the same command
- **Type Safety** - Strongly-typed parameters with compile-time checking
- **Zero Dependencies** - Lightweight with no external dependencies

## Code Quality
[![Sonar Analysis](https://github.com/huneriann/SharpCLI/actions/workflows/sonar-analyze.yml/badge.svg)](https://github.com/huneriann/SharpCLI/actions/workflows/sonar-analyze.yml)
[![Quality Gate Status](https://img.shields.io/sonar/quality_gate/huneriann_SharpCLI?server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/summary/new_code?id=huneriann_SharpCLI)
[![Coverage](https://img.shields.io/sonar/coverage/huneriann_SharpCLI?server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/component_measures?id=huneriann_SharpCLI&metric=coverage&view=list)

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-dark.svg)](https://sonarcloud.io/summary/new_code?id=huneriann_SharpCLI)

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

## Builder Options

The SharpCliHostBuilder provides a clean, fluent way to configure your application:

```csharp

var app = new SharpCliHost()
    .CreateBuilder()                        
    .Name("myapp")                          // Required: App name used in help/usage
    .Description("My awesome CLI tool")     // Optional: Shown in main help header
    .Writer(new StringWriter())             // Optional: Custom output (great for testing)
    .CustomHelpMessage("Custom welcome text\nLine two...") // Optional: Top of main help
    .Build();
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

## Performance

SharpCLI is built with a focus on low overhead and high-speed command dispatching. Below is a benchmark demonstrating the execution speed of a complex command involving mixed arguments and options.

### Benchmark Context

```csharp
var host = new SharpCliHost("BenchmarkSample", "Benchmarking SharpCliHost");
host.RegisterCommands<SampleStaticCommands>();

string[] args = ["mixed", "test", "42", "--flag", "--value", "custom"];
await host.RunAsync(args);

public class SampleStaticCommands : ICommandsContainer
{
    [Command("mixed", Description = "Mixed arguments and options")]
    public static void MixedCommand(
        string arg1,
        [Argument("arg2", Required = false)] int arg2 = 0,
        [Option("f", "flag")] bool flag = false,
        [Option("v", "value", Description = "A string value")] string value = "default")
    {
        // Method body
    }
}
```

### Benchmark Result

| Method | Runtime | Mean | Error | StdDev | Allocated | Rank |
|:---|:---|:---|:---|:---|:---|:---:|
| **Run Mixed Args and Options** | .NET 10.0 | 93,923.70 ns | 1,617.63 ns | 1,986.60 ns | 1192 B | 1 |


Detailed benchmarking reports and environment details can be found in [BENCHMARK.md](./BENCHMARK.md).

## License

This project is licensed under the MIT License - see
the [MIT License](https://github.com/huneriann/sharpcli/blob/master/LICENSE.md) file for details. 
