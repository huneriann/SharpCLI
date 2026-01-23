namespace SharpCLI.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Commands;
using Configurations;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[Config(typeof(LimitedIterationsConfig))]
[SimpleJob(RuntimeMoniker.Net10_0, baseline: true)]
public class SharpCliHostBenchmarks
{
    private SharpCliHost _host;
    private SampleInstanceCommands _instanceCommands;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _host = new SharpCliHost("BenchmarkApp", "Benchmarking SharpCliHost");
        _instanceCommands = new SampleInstanceCommands();

        // Register static commands
        _host.RegisterCommands<SampleStaticCommands>();

        // Register instance commands
        _host.RegisterCommands(_instanceCommands);
    }

    [Benchmark(Description = "Register Static Commands")]
    public SharpCliHost BenchmarkRegisterStaticCommands()
    {
        var newHost = new SharpCliHost("TestApp");
        return newHost.RegisterCommands<SampleStaticCommands>();
    }

    [Benchmark(Description = "Register Instance Commands")]
    public SharpCliHost BenchmarkRegisterInstanceCommands()
    {
        var newHost = new SharpCliHost("TestApp");
        var newInstance = new SampleInstanceCommands();
        return newHost.RegisterCommands(newInstance);
    }

    [Benchmark(Description = "Run Simple Positional Command")]
    public async Task<int> BenchmarkRunSimpleCommand()
    {
        string[] args = { "hello", "world" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run Command with Alias")]
    public async Task<int> BenchmarkRunWithAlias()
    {
        string[] args = { "hi", "world" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run Sync Command with Options")]
    public async Task<int> BenchmarkRunWithOptions()
    {
        string[] args = { "options-test", "--verbose", "-c", "5" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run Async Command")]
    public async Task<int> BenchmarkRunAsyncCommand()
    {
        string[] args = { "async-task", "Hello async" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run Mixed Args and Options")]
    public async Task<int> BenchmarkRunMixed()
    {
        string[] args = { "mixed", "test", "42", "--flag", "--value", "custom" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run with Enum Argument")]
    public async Task<int> BenchmarkRunWithEnum()
    {
        string[] args = { "enum-test", "Red" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Run with Many Positional Args")]
    public async Task<int> BenchmarkRunManyArgs()
    {
        string[] args = { "many-args", "a", "b", "c", "d", "e" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Show Global Help")]
    public async Task<int> BenchmarkShowGlobalHelp()
    {
        string[] args = { "--help" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Show Command Help")]
    public async Task<int> BenchmarkShowCommandHelp()
    {
        string[] args = { "mixed", "--help" };
        return await _host.RunAsync(args);
    }

    [Benchmark(Description = "Get Default Value (Int)")]
    public object BenchmarkGetDefaultInt()
    {
        return typeof(SharpCliHost).GetMethod("GetDefaultValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { typeof(int) });
    }

    [Benchmark(Description = "Convert Value (String to Int)")]
    public object BenchmarkConvertToInt()
    {
        return typeof(SharpCliHost).GetMethod("ConvertValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { "42", typeof(int), "test" });
    }

    [Benchmark(Description = "Convert Value (String to Enum)")]
    public object BenchmarkConvertToEnum()
    {
        return typeof(SharpCliHost).GetMethod("ConvertValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { "Red", typeof(ConsoleColor), "color" });
    }
}