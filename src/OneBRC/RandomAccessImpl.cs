using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class RandomAccessImpl
{
    private readonly string _filePath;

    public RandomAccessImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var aggregate = new Dictionary<Key, Accumulator>();
        var fileHandle = File.OpenHandle(_filePath);

        long offset = 0;

        var buffer = new byte[1024];

        int read = RandomAccess.Read(fileHandle, buffer, offset);
        int spanLength = read;

        while (spanLength > 0)
        {
            start:
            var span = buffer.AsSpan(0, spanLength);

            while (span.Length > 0)
            {
                var endOfLine = span.IndexOf((byte)'\n');

                if (endOfLine < 0)
                {
                    span.CopyTo(buffer);
                    var readSpan = buffer.AsSpan(span.Length);
                    offset += read;
                    read = RandomAccess.Read(fileHandle, readSpan, offset);
                    spanLength = span.Length + read;
                    goto start;
                }

                var line = span.Slice(0, endOfLine);
                ProcessLine(line, aggregate);

                span = span.Slice(endOfLine + 1);
            }

            offset += read;
            spanLength = read = RandomAccess.Read(fileHandle, buffer, offset);
        }

        var returnValue = aggregate.Values.ToDictionary(v => v.City);

        return new ValueTask<Dictionary<string, Accumulator>>(returnValue);
    }

    private static void ProcessLine(Span<byte> span, Dictionary<Key, Accumulator> aggregate)
    {
        var semicolon = span.IndexOf((byte)';');

        var name = span.Slice(0, semicolon);

        var key = KeyHash.ToKey(name);

        if (!aggregate.TryGetValue(key, out var accumulator))
        {
            aggregate[key] = accumulator = new(Encoding.UTF8.GetString(name));
        }

        var reading = span.Slice(semicolon + 1);
        var value = float.Parse(reading);

        accumulator.Record(value);
    }
}