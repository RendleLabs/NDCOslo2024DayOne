using System.Globalization;
using System.Runtime.CompilerServices;

namespace OneBRC;

public static class FloatAsInt
{
    private static readonly byte Separator = (byte)CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parse(in ReadOnlySpan<byte> span)
    {
        bool negative = span[0] == (byte)'-';

        int value = 0;

        for (int i = negative ? 1 : 0, l = span.Length; i < l; i++)
        {
            if (span[i] != Separator)
            {
                value = (value * 10) + (span[i] - 48);
            }
        }

        return negative ? -value : value;
    }
}