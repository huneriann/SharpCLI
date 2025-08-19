namespace SharpCLI;

/// <summary>
/// Marker interface for classes containing CLI command methods.
/// </summary>
/// <remarks>
/// Implement this interface on classes that contain methods decorated with [Command] attributes.
/// This prevents accidentally passing Type objects instead of class instances.
/// </remarks>
/// <example>
/// <code>
/// public class MyCommands : ICommandContainer
/// {
///     [Command("hello")]
///     public void SayHello() => Console.WriteLine("Hello!");
/// }
/// 
/// host.RegisterCommands(new MyCommands());
/// </code>
/// </example>
public interface ICommandsContainer;