using System.Reflection;

namespace OneBRC.Tests;

public class IntegerMemoryMappedFileTests
{
    private static readonly string FilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "million.txt"
    );
    
    [Fact]
    public void Works()
    {
        var impl = new MemoryMappedFileIntegerImpl(FilePath);
        var actual = impl.Run().ToArray();
        Assert.NotEmpty(actual);
    }
}