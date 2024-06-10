using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class StreamImpl
{
    private readonly string _filePath;

    public StreamImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        int count = 0;
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

                if (++count % 1_000_000 == 0)
                {
                    Console.WriteLine(count);
                }
            }

            read = stream.Read(buffer);
        }


        foreach (var accumulator in aggregate.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }

        return default;
    }

    private static void ProcessLine(Span<byte> span, Dictionary<string, Accumulator> aggregate)
    {
        var semicolon = span.IndexOf((byte)';');
        var name = span.Slice(0, semicolon);
        var reading = span.Slice(semicolon + 1);

        var key = Encoding.UTF8.GetString(name);
        try
        {
            var value = float.Parse(reading);
            if (!aggregate.TryGetValue(key, out var accumulator))
            {
                aggregate[key] = accumulator = new Accumulator(key);
            }

            accumulator.Record(value);
        }
        catch
        {
            var actual = Encoding.UTF8.GetString(reading);
            Console.WriteLine($"Broke on {actual}");
            throw;
        }
    }
}