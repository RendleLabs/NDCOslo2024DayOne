using System.Reflection;

namespace OneBRC.Tests;

public class RandomAccessMultiThreadedTests
{
    private static readonly string FilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "million.txt"
    );
    
    [Fact]
    public void Works()
    {
        var impl = new RandomAccessMultiThreadedImpl(FilePath);
        var actual = impl.Run().Result;
        Assert.NotEmpty(actual);
    }
}