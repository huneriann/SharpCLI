namespace SharpCLI.Models;

using System;

/// <summary>
/// Represents metadata for a single parameter of a CLI command method.
/// This class contains all information needed to parse command-line input
/// and map it to the corresponding method parameter, supporting both
/// positional arguments and named options.
/// </summary>
internal class ParameterInfo
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// For arguments, this is the descriptive name shown in help text.
    /// For options, this is typically the method parameter name.
    /// </summary>
    /// <example>
    /// Arguments: "filename", "destination"
    /// Options: "timeout", "verbose"
    /// </example>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the .NET type that this parameter expects.
    /// Used for type conversion from command-line strings to strongly-typed values.
    /// </summary>
    /// <example>
    /// typeof(string), typeof(int), typeof(bool), typeof(FileInfo)
    /// </example>
    public Type Type { get; set; } = typeof(string);

    /// <summary>
    /// Gets or sets a value indicating whether this parameter is a positional argument.
    /// Arguments are specified by position and do not use flags (no - or --).
    /// </summary>
    /// <example>
    /// In "myapp copy source.txt dest.txt", both "source.txt" and "dest.txt" are arguments.
    /// </example>
    public bool IsArgument { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this parameter is a named option.
    /// Options are specified with flags and can appear in any order.
    /// </summary>
    /// <example>
    /// In "myapp build --output bin --verbose", both "--output" and "--verbose" are options.
    /// </example>
    public bool IsOption { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this parameter must be provided by the user.
    /// Required parameters will cause an error if not specified.
    /// </summary>
    /// <remarks>
    /// Arguments are typically required by default, while options are usually optional.
    /// The framework validates required parameters during argument parsing.
    /// </remarks>
    public bool Required { get; set; } = true;

    /// <summary>
    /// Gets or sets the default value to use when the parameter is not provided.
    /// This value is used if the parameter is optional and not specified by the user.
    /// </summary>
    /// <example>
    /// For an optional "--timeout 30" parameter, DefaultValue might be 30.
    /// For a "--verbose" flag, DefaultValue would be false.
    /// </example>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the human-readable description of what this parameter does.
    /// This description is displayed in help text to guide users.
    /// </summary>
    /// <example>
    /// "The source file to copy", "Enable verbose output", "Connection timeout in seconds"
    /// </example>
    public string Description { get; set; } = "";

    //
    // Argument-specific properties
    //

    /// <summary>
    /// Gets or sets the zero-based position of this argument in the command line.
    /// Only used when <see cref="IsArgument"/> is true.
    /// Determines the order in which arguments must be specified.
    /// </summary>
    /// <example>
    /// In "myapp copy source dest", "source" has Position=0, "dest" has Position=1.
    /// </example>
    public int Position { get; set; }

    //
    // Option-specific properties
    //

    /// <summary>
    /// Gets or sets the short form flag for this option (single character).
    /// Only used when <see cref="IsOption"/> is true.
    /// Allows users to specify options with a single dash and letter.
    /// </summary>
    /// <example>
    /// For a "--verbose" option, ShortName might be "v" allowing "-v".
    /// For a "--output" option, ShortName might be "o" allowing "-o filename".
    /// </example>
    public string ShortName { get; set; } = "";

    /// <summary>
    /// Gets or sets the long form flag for this option (full word).
    /// Only used when <see cref="IsOption"/> is true.
    /// Provides a descriptive name for the option using double dashes.
    /// </summary>
    /// <example>
    /// "verbose" for "--verbose", "output" for "--output", "help" for "--help"
    /// </example>
    public string LongName { get; set; } = "";
}