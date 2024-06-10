using System.IO.MemoryMappedFiles;
using System.Text;
using OneBRC.Shared;

namespace OneBRC;

internal class MemoryParser
{
    private const byte NewLine = (byte)'\n';
    private const byte Semicolon = (byte)';';

    private readonly Dictionary<Key, Accumulator> _dictionary = new();
    private readonly MemoryMappedViewAccessor _viewAccessor;
    private readonly MemoryMappedFileOffset _fileOffset;

    public MemoryParser(MemoryMappedViewAccessor viewAccessor, MemoryMappedFileOffset fileOffset)
    {
        _viewAccessor = viewAccessor;
        _fileOffset = fileOffset;
    }

    public Dictionary<Key, Accumulator> Dictionary => _dictionary;

    public unsafe void Run()
    {
        byte* pointer = null;
        _viewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
        pointer += _fileOffset.Offset;

        const int chunkSize = 1024 * 1024 * 16;

        long remaining = _fileOffset.Length;

        while (remaining > 0)
        {
            var spanLength = (int)Math.Min(remaining, chunkSize);
            ReadOnlySpan<byte> span = new Span<byte>(pointer, spanLength);
            if (span.Length > remaining)
            {
                span = span[..(int)remaining];
                remaining = 0L;
            }

            Run(ref span, _dictionary);

            int advance = chunkSize - span.Length;
            remaining -= advance;
            pointer += advance;
        }
    }

    private static void Run(ref ReadOnlySpan<byte> span, Dictionary<Key, Accumulator> dictionary)
    {
        int newline;

        while ((newline = span.IndexOf(NewLine)) > -1)
        {
            var line = span[..newline];
            int sc = line.IndexOf(Semicolon);
            if (sc < 1)
            {
                span = span[(newline + 1)..];
                continue;
            }
            
            var number = line[(sc + 1)..];
            var value = float.Parse(number);
            
            var name = line[..sc];
            var key = KeyHash.ToKey(name);

            if (!dictionary.TryGetValue(key, out var accumulator))
            {
                dictionary[key] = accumulator = new Accumulator(Encoding.UTF8.GetString(name));
            }
            
            accumulator.Record(value);
            span = span.Slice(newline + 1); // [(newline + 1)..];
        }
    }
}