using System.Diagnostics;
using OneBRC;

Console.WriteLine("Running...");

Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

// var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);
var impl = new MemoryMappedFileIntegerImpl(filePath);
var accumulators = impl.Run();

// if (!task.IsCompleted)
// {
//     await task;
// }
//
// var dictionary = task.Result;

foreach (var accumulator in accumulators.OrderBy(a => a.City))
{
    Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
}

// stopwatch.Stop();
//
// Console.WriteLine();
// Console.WriteLine($"Done in {stopwatch.Elapsed:g}");
// Console.WriteLine();
