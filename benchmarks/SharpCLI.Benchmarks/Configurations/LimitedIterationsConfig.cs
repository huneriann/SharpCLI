namespace SharpCLI.Benchmarks.Configurations;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

public class LimitedIterationsConfig : ManualConfig
{
    public LimitedIterationsConfig()
    {
        AddJob(Job.Default.WithIterationCount(10));
    }
}