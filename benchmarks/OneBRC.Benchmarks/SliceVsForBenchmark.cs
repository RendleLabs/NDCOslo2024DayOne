using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class SliceVsForBenchmark
{
    private static readonly ReadOnlyMemory<byte> Number = "1982749"u8.ToArray();
    
    [Benchmark]
    public int Slice()
    {
        var span = Number.Span;
        int value = 0;
        while (span.Length > 0)
        {
            value = (value * 10) + (span[0] - 48);
            span = span.Slice(1);
        }

        return value;
    }

    [Benchmark]
    public int Range()
    {
        var span = Number.Span;
        int value = 0;
        while (span.Length > 0)
        {
            value = (value * 10) + (span[0] - 48);
            span = span[1..];
        }

        return value;
    }

    [Benchmark(Baseline = true)]
    public int For()
    {
        var span = Number.Span;
        int value = 0;
        for (int i = 0; i < span.Length; i++)
        {
            value = (value * 10) + (span[i] - 48);
        }

        return value;
    }
}