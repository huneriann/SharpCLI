namespace SharpCLI;

using System;
using System.IO;

/// <summary>
/// Provides a fluent builder for constructing a <see cref="SharpCliHost"/> instance.
/// Allows configuration of application name, description, output writer, and a custom help banner message.
/// </summary>
/// <remarks>
/// Use <see cref="SharpCliHost.CreateBuilder"/> to obtain an instance of this builder.
/// The application name is required; all other properties are optional.
/// </remarks>
public class SharpCliHostBuilder
{
    private string? _name;
    private string _description = "";
    private TextWriter? _writer;
    private string? _customHelpMessage;

    /// <summary>
    /// Sets the name of the CLI application.
    /// This name appears in usage lines and help output.
    /// </summary>
    /// <param name="name">The application name.</param>
    /// <returns>The current <see cref="SharpCliHostBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
    public SharpCliHostBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the optional description of the CLI application.
    /// Displayed at the top of the main help screen.
    /// </summary>
    /// <param name="description">The application description.</param>
    /// <returns>The current <see cref="SharpCliHostBuilder"/> instance for method chaining.</returns>
    public SharpCliHostBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets a custom <see cref="TextWriter"/> for output.
    /// Useful for redirecting output during testing or logging.
    /// If not set, <see cref="Console.Out"/> is used by default.
    /// </summary>
    /// <param name="writer">The writer to use for all output.</param>
    /// <returns>The current <see cref="SharpCliHostBuilder"/> instance for method chaining.</returns>
    public SharpCliHostBuilder Writer(TextWriter? writer)
    {
        _writer = writer;
        return this;
    }

    /// <summary>
    /// Sets a custom help message to display at the top of the main help screen.
    /// This message appears before the standard usage information and command list.
    /// Useful for welcome messages, version info, or custom instructions.
    /// </summary>
    /// <param name="message">
    /// The custom message text. Can be multiline. Set to null or empty to disable.
    /// </param>
    /// <returns>The current <see cref="SharpCliHostBuilder"/> instance for method chaining.</returns>
    public SharpCliHostBuilder CustomHelpMessage(string? message)
    {
        _customHelpMessage = message;
        return this;
    }

    /// <summary>
    /// Builds and returns a fully configured <see cref="SharpCliHost"/> instance.
    /// </summary>
    /// <returns>A new <see cref="SharpCliHost"/> instance with the specified configuration.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the application name has not been set via <see cref="Name(string)"/>.
    /// </exception>
    public SharpCliHost Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            throw new InvalidOperationException("Application name must be set using .Name()");
        }

        return new SharpCliHost(_name, _description, _writer, _customHelpMessage);
    }
}