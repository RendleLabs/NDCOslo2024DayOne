using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class LoopBenchmark
{
    private static readonly List<string> Linebreakers =
    [
        "Dylan", "Vagif", "Hannes", "Heather", "Mark", "Eli",
    ];

    [Benchmark(Baseline = true)]
    public int Foreach()
    {
        int n = 0;
        
        foreach (var x in Linebreakers)
        {
            n += x.Length;
        }

        return n;
    }
    
    [Benchmark]
    public int For()
    {
        int n = 0;
        
        for (int i = 0, l = Linebreakers.Count; i < l; i++)
        {
            n += Linebreakers[i].Length;
        }

        return n;
    }
    
    [Benchmark]
    public int ForeachSpan()
    {
        int n = 0;

        var span = CollectionsMarshal.AsSpan(Linebreakers);
        
        foreach (var x in span)
        {
            n += x.Length;
        }

        return n;
    }
    
    [Benchmark]
    public int ForSpan()
    {
        int n = 0;

        var span = CollectionsMarshal.AsSpan(Linebreakers);
        
        for (int i = 0, l = span.Length; i < l; i++)
        {
            n += span[i].Length;
        }

        return n;
    }

    [Benchmark]
    public int LinqSum() => Linebreakers.Sum(l => l.Length);
}