using CsvHelper.Configuration.Attributes;

namespace iso_bench_to_tikz;

// [Delimiter(",")]
[Delimiter(";")]
public class BenchmarkResult
{
    public string Method { get; set; }
    public string Job { get; set; }
    public string AnalyzeLaunchVariance { get; set; }
    public string EvaluateOverhead { get; set; }
    public string MaxAbsoluteError { get; set; }
    public string MaxRelativeError { get; set; }
    public string MinInvokeCount { get; set; }
    public string MinIterationTime { get; set; }
    public string OutlierMode { get; set; }
    public string Affinity { get; set; }
    public string EnvironmentVariables { get; set; }
    public string Jit { get; set; }
    public string Platform { get; set; }
    public string PowerPlanMode { get; set; }
    public string Runtime { get; set; }
    public string AllowVeryLargeObjects { get; set; }
    public string Concurrent { get; set; }
    public string CpuGroups { get; set; }
    public string Force { get; set; }
    public string HeapAffinitizeMask { get; set; }
    public string HeapCount { get; set; }
    public string NoAffinitize { get; set; }
    public string RetainVm { get; set; }
    public string Server { get; set; }
    public string Arguments { get; set; }
    public string BuildConfiguration { get; set; }
    public string Clock { get; set; }
    public string EngineFactory { get; set; }
    public string NuGetReferences { get; set; }
    public string Toolchain { get; set; }
    public string IsMutator { get; set; }
    public string InvocationCount { get; set; }
    public string IterationCount { get; set; }
    public string IterationTime { get; set; }
    public string LaunchCount { get; set; }
    public string MaxIterationCount { get; set; }
    public string MaxWarmupIterationCount { get; set; }
    public string MemoryRandomization { get; set; }
    public string MinIterationCount { get; set; }
    public string MinWarmupIterationCount { get; set; }
    public string RunStrategy { get; set; }
    public string UnrollFactor { get; set; }
    public string WarmupCount { get; set; }
    public string Pair { get; set; }
    public string Mean { get; set; }
    public string Error { get; set; }
    [Optional]
    public string? StdDev { get; set; }
    [Optional]
    public string? Median { get; set; }
    [Optional]
    public string? Ratio { get; set; }
    [Optional]
    public string? RatioSD { get; set; }
}