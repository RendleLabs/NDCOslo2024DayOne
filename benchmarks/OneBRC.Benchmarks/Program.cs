using System.Reflection;
using BenchmarkDotNet.Running;
using OneBRC.Benchmarks;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
