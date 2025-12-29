# Contributing to SharpCLI

Thank you for your interest in contributing to SharpCLI! We welcome contributions from everyone, whether you're fixing
bugs, adding features, improving documentation, or helping with testing.

## Getting Started

### Prerequisites

- **.NET Standard 2.0/2.1 or .NET 5.0 SDK** or later
- **Git**
- A code editor (Visual Studio, VS Code, JetBrains Rider, etc.)

### Setting Up the Development Environment

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```
   git clone https://github.com/your-username/SharpCLI.git
   cd SharpCLI
   ```
3. **Add the upstream remote**:
   ```
   git remote add upstream https://github.com/huneriann/SharpCLI.git
   ```
4. **Restore dependencies**:
   ```
   dotnet restore
   ```
5. **Build the project**:
   ```
   dotnet build
   ```

## Project Structure

```
SharpCLI/
├── src/
│   └── SharpCLI/                 # Main library project
│       ├── Attributes/           # Command, Argument, Option attributes
│       ├── Models/              # Internal data models
│       ├── SharpCliHost.cs      # Main framework class
│       └── SharpCLI.csproj      # Multi-target (netstandard2.0, net8.0)
├── .github/workflows/           # CI/CD workflows
├── README.md
├── CONTRIBUTING.md
└── LICENSE.md
```

## How to Contribute

### Reporting Issues

Before creating a new issue, please:

1. **Search existing issues** to avoid duplicates
2. **Use the issue template** if available
3. **Provide clear reproduction steps** for bugs
4. **Include relevant information**:
    - .NET version and target framework
    - Operating system
    - SharpCLI version
    - Code samples demonstrating the issue

### Suggesting Features

We welcome feature requests! Please:

1. **Check existing feature requests** first
2. **Describe the use case** and why the feature would be valuable
3. **Provide examples** of how the feature would be used
4. **Consider the impact** on existing APIs and backward compatibility

### Contributing Code

#### Branch Naming Convention

Use descriptive branch names with prefixes:

- `feature/` - New features
- `bugfix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Test improvements
- `ci/` - CI/CD changes

Examples:

- `feature/add-configuration-file-support`
- `bugfix/fix-nullable-parameter-parsing`
- `docs/update-api-documentation`

#### Commit Message Guidelines

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Types:**

- `feat` - New features
- `fix` - Bug fixes
- `docs` - Documentation changes
- `style` - Code style changes (formatting, etc.)
- `refactor` - Code refactoring
- `test` - Adding or updating tests
- `ci` - CI/CD changes
- `perf` - Performance improvements

**Examples:**

```
feat: add support for configuration files
fix(parsing): handle nullable enum parameters correctly
docs: update README with async command examples
test: add unit tests for multi-target framework compatibility
```

#### Pull Request Process

1. **Create a feature branch** from `master`:
   ```
   git checkout master
   git pull upstream master
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following our coding standards

3. **Build and test** across all target frameworks:
   ```
   dotnet build
   dotnet test  # When tests are available
   ```

4. **Update documentation** if needed (README.md, CONTRIBUTING.md etc.)

5. **Commit your changes** with clear commit messages

6. **Push to your fork**:
   ```
   git push origin feature/your-feature-name
   ```

7. **Create a Pull Request** with:
    - Clear title and description
    - Reference to related issues
    - Description of changes made
    - Any breaking changes noted

#### Code Review Process

- All submissions require review before merging
- We may ask for changes or improvements
- Please respond to feedback promptly
- Once approved, maintainers will merge your PR

## Coding Standards

### C# Style Guidelines

Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add **comprehensive XML documentation** for public APIs
- Keep methods focused and concise
- Use **nullable reference types** appropriately

### Multi-Target Framework Support

SharpCLI targets both **netstandard2.0** and **net8.0**:

- **netstandard2.0/2.1**: Broad compatibility (.NET Framework 4.7.2+, .NET Core 2.0+)
- **net5.0**: Modern features

Use conditional compilation when needed:

```csharp
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
```

### Code Quality

- **Maintain backward compatibility** - breaking changes require major version bump
- **Handle edge cases** and error conditions gracefully
- **Use async/await** properly for async operations
- **Follow SOLID principles**
- **Maintain trimming safety** for AOT scenarios

### Performance Considerations

- **Avoid allocations** in hot paths
- **Use caching** where appropriate (like DefaultValueFactories)
- **Consider trimming** and AOT compilation impact
- **Profile performance-critical code**

### Example Code Style

```csharp
namespace SharpCLI;

/// <summary>
/// Represents configuration for a CLI command with validation.
/// </summary>
/// <param name="name">The command name</param>
/// <param name="description">Optional command description</param>
public class CommandConfig(string name, string? description = null)
{
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Gets the command description.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Validates the command configuration.
    /// </summary>
    /// <returns>True if valid, false otherwise.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }
}
```

## Documentation

### API Documentation

- Use **comprehensive XML documentation** for all public APIs
- Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
- Provide **code examples** in `<example>` tags where helpful
- Document edge cases and important behavior

### README Updates

- Update examples if you change public APIs
- Add new features to the feature list
- Keep installation and usage instructions current
- Maintain compatibility with the existing style

## Release Process

Releases are handled by maintainers:

1. **Version tags** (v*) trigger automatic NuGet publishing via GitHub Actions
2. **Breaking changes** require major version increments
3. **Multi-target packages** are built and published automatically

## Architecture Guidelines

### Framework Design Principles

SharpCLI follows these core paradigms:

1. **Attribute-Driven Programming** - Use attributes as DSL for CLI definition
2. **Convention over Configuration** - Minimize boilerplate, maximize automation
3. **Performance-First** - Compiled expressions, caching, trimming-safe design
4. **Developer Experience** - Make simple things simple, complex things possible

### Key Components

- **SharpCliHost**: Main orchestrator and entry point
- **Attributes**: Declarative command definition (`[Command]`, `[Argument]`, `[Option]`)
- **Models**: Internal data structures (CommandInfo, ParameterInfo)
- **Parsing Engine**: Argument/option parsing with type conversion

## Getting Help

- **GitHub Issues** - For bugs and feature requests
- **GitHub Discussions** - For questions and general discussion
- **Code Reviews** - Learn from feedback on your PRs

## Code of Conduct

Please be respectful and professional in all interactions. We're all here to make SharpCLI better together.

## License

By contributing to SharpCLI, you agree that your contributions will be licensed under the same license as the project (
MIT License).

🎉 **Thank you for contributing to SharpCLI!** 