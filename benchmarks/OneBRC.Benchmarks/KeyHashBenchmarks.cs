using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class KeyHashBenchmarks
{
    private static readonly byte[][] Data =
    [
        "Oslo"u8.ToArray(),
        "Rio"u8.ToArray(),
        "Copenhagen"u8.ToArray(),
    ];

    [Benchmark(Baseline = true)]
    public long Simple()
    {
        unchecked
        {
            return KeyHash.ToLong(Data[0])
                   + KeyHash.ToLong(Data[1])
                   + KeyHash.ToLong(Data[2]);
        }
    }
    
    [Benchmark]
    public long TryInt()
    {
        unchecked
        {
            return KeyHash.ToLong2(Data[0])
                   + KeyHash.ToLong2(Data[1])
                   + KeyHash.ToLong2(Data[2]);
        }
    }

    [Benchmark]
    public (Key, Key, Key) Key()
    {
        return (
            KeyHash.ToKey(Data[0]),
            KeyHash.ToKey(Data[1]),
            KeyHash.ToKey(Data[2])
            );
    }
}