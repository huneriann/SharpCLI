# Changelog

All notable changes to **SharpCLI** will be documented in this file.

---

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