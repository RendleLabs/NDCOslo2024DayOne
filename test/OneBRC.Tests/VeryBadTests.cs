using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OneBRC.Tests;

public class VeryBadTests
{
    [Fact]
    public unsafe void PointerAbuse()
    {
        var value = 42;

        byte* pointer = (byte*)Unsafe.AsPointer(ref value);

        for (int i = 1; i < 4; i++)
        {
            Assert.Equal(0, *(pointer + i));
        }
        Assert.Equal(42, *pointer);

        // Do not do this! It's very bad!
        long* badPointer = (long*)pointer;
        
        Assert.Equal(42, *badPointer);
    }
}