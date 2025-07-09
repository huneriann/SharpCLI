namespace SharpCLI;

using System;

/// <summary>
/// Marks a method as a CLI command that can be invoked from the command line.
/// This attribute transforms a regular method into an executable command with
/// automatic argument parsing, help generation, and error handling.
/// </summary>
/// <remarks>
/// <para>Only methods marked with this attribute will be discovered and registered as commands.</para>
/// <para>The method can be static or instance, synchronous or asynchronous (Task&lt;int&gt; or Task).</para>
/// <para>Method parameters can be decorated with [Argument] or [Option] attributes to define CLI behavior.</para>
/// </remarks>
/// <example>
/// <code>
/// [Command("build", Description = "Build the project", Aliases = ["b", "compile"])]
/// public static int BuildProject(
///     [Argument("project", Description = "Project file to build")] string projectFile,
///     [Option("o", "output", Description = "Output directory")] string output = "bin")
/// {
///     // Build implementation
///     return 0; // Success
/// }
/// 
/// // Usage examples:
/// // myapp build project.csproj
/// // myapp b project.csproj --output dist
/// // myapp compile project.csproj -o dist
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the primary name of the command as it will appear in the CLI.
    /// This is the main identifier users will type to invoke the command.
    /// </summary>
    /// <remarks>
    /// <para>Command names should be:</para>
    /// <list type="bullet">
    /// <item>Lowercase and descriptive</item>
    /// <item>Use hyphens for multi-word commands (e.g., "build-all")</item>
    /// <item>Unique within the application</item>
    /// <item>Short but meaningful</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// "build", "deploy", "test", "clean", "install", "version"
    /// </example>
    public string Name { get; } = name;

    /// <summary>
    /// Gets or sets a brief description of what the command does.
    /// This description appears in the main help listing and command-specific help.
    /// </summary>
    /// <remarks>
    /// <para>Good descriptions should:</para>
    /// <list type="bullet">
    /// <item>Be concise (one sentence preferred)</item>
    /// <item>Start with a verb in present tense</item>
    /// <item>Clearly explain the command's purpose</item>
    /// <item>Avoid technical jargon when possible</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// "Build the project and generate output files",
    /// "Deploy application to the specified environment",
    /// "Run unit tests and generate coverage report"
    /// </example>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets an array of alternative names (aliases) for the command.
    /// Aliases allow users to invoke the same command using shorter or alternative names,
    /// improving user experience and accommodating different preferences.
    /// </summary>
    /// <remarks>
    /// <para>Common alias patterns:</para>
    /// <list type="bullet">
    /// <item>Single letter shortcuts (e.g., "b" for "build")</item>
    /// <item>Alternative verbs (e.g., "compile" for "build")</item>
    /// <item>Abbreviated forms (e.g., "ver" for "version")</item>
    /// <item>Legacy command names for backward compatibility</item>
    /// </list>
    /// <para>All aliases must be unique across the entire application.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Build command with multiple aliases
    /// [Command("build", Aliases = ["b", "compile", "make"])]
    /// 
    /// // Version command with common shortcuts
    /// [Command("version", Aliases = ["v", "ver", "--version"])]
    /// 
    /// // Help command with standard variations
    /// [Command("help", Aliases = ["h", "?", "--help"])]
    /// </code>
    /// </example>
    public string[] Aliases { get; set; } = [];
}