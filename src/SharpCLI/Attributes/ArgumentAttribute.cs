namespace SharpCLI;

using System;

/// <summary>
/// Marks a method parameter as a positional command-line argument.
/// Arguments are specified by their position in the command line and do not use flags.
/// They represent the main data that the command operates on.
/// </summary>
/// <remarks>
/// Arguments are processed in the order they appear in the method signature.
/// The first argument gets the first non-option value from the command line,
/// the second argument gets the second value, and so on.
/// </remarks>
/// <example>
/// <code>
/// [Command("copy")]
/// public static int CopyFile(
///     [Argument("source", Description = "Source file path")] string source,
///     [Argument("destination", Description = "Destination file path")] string destination)
/// {
///     // Implementation
/// }
/// 
/// // Usage: myapp copy file1.txt file2.txt
/// // "file1.txt" → source parameter
/// // "file2.txt" → destination parameter
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter)]
public class ArgumentAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the descriptive name of the argument.
    /// This name is used in help text and error messages to identify the argument.
    /// </summary>
    /// <remarks>
    /// The name should be descriptive and indicate what kind of data is expected.
    /// It appears in help text as uppercase (e.g., "SOURCE", "FILENAME").
    /// </remarks>
    /// <example>
    /// "filename", "url", "destination", "count"
    /// </example>
    public string Name { get; } = name;

    /// <summary>
    /// Gets or sets a human-readable description of what this argument represents.
    /// This description is displayed in the help text to guide users on what to provide.
    /// </summary>
    /// <example>
    /// "The source file to copy", "URL to download from", "Number of iterations to perform"
    /// </example>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether this argument must be provided by the user.
    /// When true, the command will fail with an error if this argument is not specified.
    /// </summary>
    /// <remarks>
    /// <para>Arguments are required by default (true) since they typically represent essential input data.</para>
    /// <para>Set to false for optional arguments that have meaningful defaults or can be omitted.</para>
    /// <para>Required arguments should appear before optional ones in the method signature.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Required argument - must be provided
    /// [Argument("filename", Required = true)] string filename
    /// 
    /// // Optional argument - can be omitted
    /// [Argument("output", Required = false)] string output = "default.txt"
    /// </code>
    /// </example>
    public bool Required { get; set; } = true;
}