using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class FileReadBenchmarks
{
    private static readonly string FilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "million.txt"
    );

    [Benchmark(Baseline = true)]
    public int StreamReader()
    {
        var impl = new StreamReaderImpl(FilePath);
        var d = impl.Run().Result;
        return d.Count;
    }

    [Benchmark]
    public int Stream()
    {
        var impl = new StreamImpl(FilePath);
        var d = impl.Run().Result;
        return d.Count;
    }

    // [Benchmark]
    public int StreamWithStringPool()
    {
        var impl = new StreamStringPoolImpl(FilePath);
        var d = impl.Run().Result;
        return d.Count;
    }

    [Benchmark]
    public int StreamLongKey()
    {
        var impl = new StreamLongKeyImpl(FilePath);
        var d = impl.Run().Result;
        return d.Count;
    }

    [Benchmark]
    public int StreamKeyKey()
    {
        var impl = new StreamKeyKeyImpl(FilePath);
        var d = impl.Run().Result;
        return d.Count;
    }
}