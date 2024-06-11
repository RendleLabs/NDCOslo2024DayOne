using System.Buffers.Text;
using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class TrimUpperBenchmarks
{
    private const string Str = "  Hello, world!    ";

    [Benchmark(Baseline = true)]
    public string ToUpperAndTrim() => Str.ToUpper().Trim();

    [Benchmark]
    public string UsingSpan()
    {
        var span = Str.AsSpan();
        return span.TrimToUpper();
    }
}

internal static class TrimUpperEx
{
    public static string TrimToUpper(ref this ReadOnlySpan<char> str)
    {
        str = str.Trim();
        Span<char> upper = stackalloc char[str.Length];
        str.ToUpper(upper, CultureInfo.InvariantCulture);
        return new string(upper);
    }

    public static string Base64ify(string input)
    {
        var byteCount = Encoding.UTF8.GetByteCount(input);
        var utf8Length = Base64.GetMaxEncodedToUtf8Length(byteCount);

        Span<byte> utf8 = stackalloc byte[utf8Length];
        byteCount = Encoding.UTF8.GetBytes(input, utf8);
        Base64.EncodeToUtf8InPlace(utf8, byteCount, out utf8Length);

        var charCount = Encoding.UTF8.GetCharCount(utf8.Slice(0, utf8Length));
        Span<char> chars = stackalloc char[charCount];
        Encoding.UTF8.GetChars(utf8.Slice(0, utf8Length), chars);
        return new string(chars);
    }

    public static string Join(string[] strs)
    {
        var length = strs.Select(s => s.Length).Sum() + strs.Length - 1;
        
        return string.Create(length, strs, (span, a) =>
        {
            for (int i = 0, l = a.Length; i < l; i++)
            {
                a[i].CopyTo(span);
                span = span.Slice(a.Length);
                span[0] = ',';
                span = span.Slice(1);
            }
        });
    }
}