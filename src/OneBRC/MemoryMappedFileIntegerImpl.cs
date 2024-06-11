using System.IO.MemoryMappedFiles;
using OneBRC.Shared;

namespace OneBRC;

public class MemoryMappedFileIntegerImpl
{
    private readonly string _filePath;

    public MemoryMappedFileIntegerImpl(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<IntAccumulator> Run()
    {
        var size = new FileInfo(_filePath).Length;
        
        var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        var offsets = MemoryMappedFileAnalyzer.GetOffsets(mmf, size, Environment.ProcessorCount);

        var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        var parsers = offsets.Select(o => new IntegerMemoryParser(view, o)).ToArray();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.ForEach(parsers, options, p => p.Run());

        var final = parsers[0].Dictionary;

        for (int i = 1; i < parsers.Length; i++)
        {
            var parser = parsers[i];
            foreach (var (key, value) in parser.Dictionary)
            {
                if (!final.TryGetValue(key, out var accumulator))
                {
                    final[key] = value;
                }
                else
                {
                    accumulator.Combine(value);
                }
            }
        }

        return final.Values;
    }
}