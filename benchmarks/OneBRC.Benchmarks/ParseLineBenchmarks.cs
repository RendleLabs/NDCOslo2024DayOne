using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class ParseLineBenchmarks
{
    private const string Line = "Oslo;11.098";
    private static readonly ReadOnlyMemory<byte> Utf8Line = "Oslo;11.098"u8.ToArray();
    
    [Benchmark(Baseline = true)]
    public float AsString()
    {
        var parts = Line.Split(';');
        var value = float.Parse(parts[1]);
        return value;
    }

    [Benchmark]
    public float AsUtf8()
    {
        var line = Utf8Line.Span;
        var semicolon = line.IndexOf((byte)';');
        var numberPart = line.Slice(semicolon + 1);
        var value = float.Parse(numberPart);
        return value;
    }
}