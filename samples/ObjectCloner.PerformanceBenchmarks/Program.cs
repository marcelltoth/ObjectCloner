using System;

using BenchmarkDotNet.Running;

namespace ObjectCloner.PerformanceBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PerformanceBenchmark>();
        }
    }
}