using System.Collections.Concurrent;
using System.Text;
using Microsoft.Win32.SafeHandles;
using OneBRC.Shared;

namespace OneBRC;

public class RandomAccessMultiThreadedImpl
{
    private readonly string _filePath;

    public RandomAccessMultiThreadedImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var fileHandle = File.OpenHandle(_filePath);

        var length = RandomAccess.GetLength(fileHandle);

        var approximateChunkSize = length / Environment.ProcessorCount;

        var offsets = new long[Environment.ProcessorCount];

        offsets[0] = 0;

        Span<byte> chunk = stackalloc byte[128];

        for (int i = 1; i < offsets.Length; i++)
        {
            var guessOffset = offsets[i - 1] + approximateChunkSize;
            var read = RandomAccess.Read(fileHandle, chunk, guessOffset);
            var newline = chunk.Slice(0, read).IndexOf((byte)'\n');
            offsets[i] = guessOffset + newline + 1;
        }

        long lastLength = length - offsets[^1];

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        var aggregate = new ConcurrentDictionary<Key, Accumulator>();

        Parallel.For(0, offsets.Length, options, i =>
        {
            if (i < Environment.ProcessorCount - 1)
            {
                ProcessChunk(fileHandle, offsets[i], offsets[i + 1] - offsets[i], aggregate);
            }
            else
            {
                ProcessChunk(fileHandle, offsets[i], lastLength, aggregate);
            }
        });

        var accumulators = aggregate.Values.ToDictionary(v => v.City!);
        return new ValueTask<Dictionary<string, Accumulator>>(accumulators);
    }

    private static void ProcessChunk(SafeFileHandle fileHandle, long offset, long length,
        ConcurrentDictionary<Key, Accumulator> aggregate)
    {
        var buffer = new byte[1024];
        long end = offset + length;
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
            if (offset > end) break;
            spanLength = read = RandomAccess.Read(fileHandle, buffer, offset);
        }
    }
    
    private static void ProcessLine(Span<byte> span, ConcurrentDictionary<Key, Accumulator> aggregate)
    {
        var semicolon = span.IndexOf((byte)';');

        var name = span.Slice(0, semicolon);

        var key = KeyHash.ToKey(name);

        var accumulator = aggregate.GetOrAdd(key, _ => new Accumulator());
        
        if (accumulator.City is null) accumulator.SetCity(Encoding.UTF8.GetString(name));
            
        var reading = span.Slice(semicolon + 1);
        var value = float.Parse(reading);

        accumulator.Record(value);
    }
}