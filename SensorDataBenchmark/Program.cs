using BenchmarkDotNet.Running;
using SensorDataBenchmark;

Console.WriteLine("Hello, World!");
BenchmarkRunner.Run<SensorDataProcessorBenchmark>();
