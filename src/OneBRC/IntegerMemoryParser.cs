using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using OneBRC.Shared;

namespace OneBRC;

internal class IntegerMemoryParser
{
    private const byte NewLine = (byte)'\n';
    private const byte Semicolon = (byte)';';

    private readonly Dictionary<Key, IntAccumulator> _dictionary = new();
    private readonly MemoryMappedViewAccessor _viewAccessor;
    private readonly MemoryMappedFileOffset _fileOffset;

    public IntegerMemoryParser(MemoryMappedViewAccessor viewAccessor, MemoryMappedFileOffset fileOffset)
    {
        _viewAccessor = viewAccessor;
        _fileOffset = fileOffset;
    }

    public Dictionary<Key, IntAccumulator> Dictionary => _dictionary;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Run(ref ReadOnlySpan<byte> span, Dictionary<Key, IntAccumulator> dictionary)
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
            var value = FloatAsInt.Parse(number);
            
            var name = line[..sc];
            var key = KeyHash.ToKey(name);

            ref var accumulator = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool existed);

            if (!existed)
            {
                accumulator.SetCity(Encoding.UTF8.GetString(name));
            }

            accumulator.Record(value);
            span = span.Slice(newline + 1); // [(newline + 1)..];
        }
    }
}