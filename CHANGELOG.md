# Changelog

All notable changes to **SharpCLI** will be documented in this file.

---

## [1.3.1] - 2025-JUL-10

### Fixed
- **Multi-target trimming compatibility** - Added conditional compilation for `DynamicallyAccessedMembers` attribute
- **IL2090 trimming warning** - Applied trimming-safe annotations only to .NET 5+ targets
- **Cross-framework compatibility** - Maintained full compatibility with netstandard2.0 without trimming warnings
- **RegisterCommands<T>() method** - Resolved trimming analysis warnings in multi-target scenarios

## [1.2.1] - 2025-JUL-10

### Fixed
- **Trimming target restriction** - Restricted trimming configuration to net8.0 target only to resolve NETSDK1212 warning
- **Target framework conditions** - Added target framework condition to trimming properties to prevent application to netstandard2.0

### Changed
- **AOT-compatible default value generation** - Replaced `Activator.CreateInstance` with compiled expression trees
- **Performance optimization** - Added thread-safe caching using `ConcurrentDictionary` for improved performance on repeated calls
- **Trimming safety** - Implemented trimming-safe default value generation using `Expression.Default` and Lambda compilation
- **Documentation** - Updated comprehensive XML documentation with examples and technical details

### Removed
- **IL2067 trimming warnings** - Eliminated trimming compatibility issues for .NET 8.0 target framework

## [1.0.0] - 2025-JUL-07

### Added
- **Initial release** of SharpCLI framework
- **Attribute-based command definition** with `[Command]`, `[Argument]`, and `[Option]` attributes
- **Automatic help generation** for commands and options
- **Comprehensive multi-target framework support**
- **Type-safe argument and option parsing**
- Support for both **instance and static command methods**
- **Command aliases** for flexible command naming
- **Async command support** with `Task<int>` and `Task` return types
- **Fluent API** for command registration
- **Built-in support** for common types:
  - `string`, `int`, `bool`, `double`, `long`, `decimal`
  - `enum` types with automatic parsing
- **Automatic default value handling**
- **Short and long option formats** (`-h`, `--help`)
- **Positional argument support** with validation
- **Exit code handling** for proper shell integration

## License

This project is licensed under the [MIT License](LICENSE.md).