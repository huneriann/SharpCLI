namespace SharpCLI;

using System;

/// <summary>
/// Marks a method parameter as a named command-line option (flag).
/// Options are specified with flags (- or --) and can appear in any order,
/// providing configuration and behavioral modifications to commands.
/// </summary>
/// <remarks>
/// <para>Options come in two forms:</para>
/// <list type="bullet">
/// <item><strong>Boolean flags:</strong> Presence indicates true (e.g., --verbose, -v)</item>
/// <item><strong>Value options:</strong> Require a value after the flag (e.g., --output file.txt, -o file.txt)</item>
/// </list>
/// <para>Options are typically optional and have reasonable defaults, unlike arguments which are usually required.</para>
/// </remarks>
/// <example>
/// <code>
/// [Command("deploy")]
/// public static int Deploy(
///     [Argument("environment")] string environment,
///     [Option("v", "verbose", Description = "Enable detailed output")] bool verbose = false,
///     [Option("t", "timeout", Description = "Connection timeout in seconds")] int timeout = 30,
///     [Option("f", "force", Description = "Force deployment without confirmation")] bool force = false)
/// {
///     // Implementation
/// }
/// 
/// // Usage examples:
/// // myapp deploy production --verbose --timeout 60
/// // myapp deploy staging -v -t 120 --force
/// // myapp deploy development -f
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter)]
public class OptionAttribute(string shortName, string longName) : Attribute
{
    /// <summary>
    /// Gets the short form flag for this option (single character with single dash).
    /// Provides a quick, abbreviated way for users to specify the option.
    /// </summary>
    /// <remarks>
    /// <para>Short names should be:</para>
    /// <list type="bullet">
    /// <item>Single character (letter or digit)</item>
    /// <item>Intuitive abbreviation of the long name</item>
    /// <item>Unique within the command</item>
    /// <item>Commonly used letters like 'v' for verbose, 'h' for help, 'o' for output</item>
    /// </list>
    /// <para>Used with single dash: -v, -o, -f</para>
    /// </remarks>
    /// <example>
    /// "v" for verbose, "o" for output, "f" for force, "t" for timeout, "h" for help
    /// </example>
    public string ShortName { get; } = shortName;

    /// <summary>
    /// Gets the long form flag for this option (full word with double dash).
    /// Provides a descriptive, self-documenting way for users to specify the option.
    /// </summary>
    /// <remarks>
    /// <para>Long names should be:</para>
    /// <list type="bullet">
    /// <item>Descriptive and self-explanatory</item>
    /// <item>Lowercase with hyphens for multi-word options</item>
    /// <item>Unique within the command</item>
    /// <item>Follow common CLI conventions</item>
    /// </list>
    /// <para>Used with double dash: --verbose, --output, --force</para>
    /// </remarks>
    /// <example>
    /// "verbose", "output", "force", "timeout", "help", "dry-run", "config-file"
    /// </example>
    public string LongName { get; } = longName;

    /// <summary>
    /// Gets or sets a human-readable description of what this option does.
    /// This description appears in help text to guide users on the option's purpose and usage.
    /// </summary>
    /// <remarks>
    /// <para>Good descriptions should:</para>
    /// <list type="bullet">
    /// <item>Clearly explain what the option does</item>
    /// <item>Mention the expected value type for value options</item>
    /// <item>Include units or format when applicable</item>
    /// <item>Be concise but informative</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// "Enable verbose output with detailed logging",
    /// "Output directory for generated files", 
    /// "Connection timeout in seconds (default: 30)",
    /// "Force operation without confirmation prompts"
    /// </example>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets the default value to use when this option is not specified by the user.
    /// This value should match the parameter's type and provide sensible behavior.
    /// </summary>
    /// <remarks>
    /// <para>Default value guidelines:</para>
    /// <list type="bullet">
    /// <item><strong>Boolean options:</strong> Usually false (option presence = true)</item>
    /// <item><strong>Numeric options:</strong> Sensible defaults like timeouts, counts, etc.</item>
    /// <item><strong>String options:</strong> Empty string, file paths, or null for optional</item>
    /// <item><strong>Enum options:</strong> Most common or safest enum value</item>
    /// </list>
    /// <para>If not set, the framework uses the parameter's default value or type defaults.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Boolean flag - defaults to false
    /// [Option("v", "verbose", DefaultValue = false)] bool verbose
    /// 
    /// // Numeric option with meaningful default
    /// [Option("t", "timeout", DefaultValue = 30)] int timeout
    /// 
    /// // String option with default path
    /// [Option("o", "output", DefaultValue = "./output")] string outputDir
    /// 
    /// // Optional string option
    /// [Option("c", "config", DefaultValue = null)] string? configFile
    /// </code>
    /// </example>
    public object? DefaultValue { get; set; }
}