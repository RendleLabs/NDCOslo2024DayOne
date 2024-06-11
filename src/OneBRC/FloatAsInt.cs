using System.Globalization;

namespace OneBRC;

public static class FloatAsInt
{
    private static readonly byte Separator = (byte)CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    public static int Parse(ReadOnlySpan<byte> span)
    {
        bool negative = span[0] == (byte)'-';
        if (negative)
        {
            span = span.Slice(1);
        }

        int value = 0;

        for (int i = 0, l = span.Length; i < l; i++)
        {
            if (span[i] != Separator)
            {
                value = (value * 10) + (span[i] - 48);
            }
        }

        return negative ? -value : value;
    }
}