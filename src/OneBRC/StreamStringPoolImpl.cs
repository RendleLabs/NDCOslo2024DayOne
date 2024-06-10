using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using OneBRC.Shared;

namespace OneBRC;

public class StreamStringPoolImpl
{
    private static readonly StringPool KeyPool = new StringPool(256);
    private readonly string _filePath;

    public StreamStringPoolImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var aggregate = new Dictionary<string, Accumulator>();

        using var stream = File.OpenRead(_filePath);

        var buffer = new byte[1024];

        int read = stream.Read(buffer);

        while (read > 0)
        {
            start:
            var span = buffer.AsSpan(0, read);

            while (span.Length > 0)
            {
                var endOfLine = span.IndexOf((byte)'\n');

                if (endOfLine < 0)
                {
                    span.CopyTo(buffer);
                    var readSpan = buffer.AsSpan(span.Length);
                    read = stream.Read(readSpan) + span.Length;
                    goto start;
                }

                var line = span.Slice(0, endOfLine);
                ProcessLine(line, aggregate);

                span = span.Slice(endOfLine + 1);
            }

            read = stream.Read(buffer);
        }

        return new ValueTask<Dictionary<string, Accumulator>>(aggregate);
    }

    private static void ProcessLine(Span<byte> span, Dictionary<string, Accumulator> aggregate)
    {
        var semicolon = span.IndexOf((byte)';');
        var name = span.Slice(0, semicolon);
        var reading = span.Slice(semicolon + 1);

        var key = KeyPool.GetOrAdd(name, Encoding.UTF8);
        
        var value = float.Parse(reading);
        if (!aggregate.TryGetValue(key, out var accumulator))
        {
            aggregate[key] = accumulator = new Accumulator(key);
        }

        accumulator.Record(value);
    }
}