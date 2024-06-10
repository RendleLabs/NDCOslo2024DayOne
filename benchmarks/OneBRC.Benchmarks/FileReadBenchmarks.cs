using System.Reflection;

namespace OneBRC.Benchmarks;

public class FileReadBenchmarks
{
    private static readonly string FilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "million.txt"
    );
    
}