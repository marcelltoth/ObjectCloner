using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectCloner.PerformanceBenchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if RELEASE
            BenchmarkDotNet.Running.BenchmarkRunner.Run<PerformanceBenchmark>();
#else
            // this block is for testing the cloning code with a profiler
            PerformanceBenchmark benchmark = new PerformanceBenchmark();
            benchmark.Original = PerformanceBenchmark.CreateTestObjects().First();
            Console.ReadKey();
            List<object> clones = new List<object>();
            for (int i = 0; i < 1000000; i++)
            {
                clones.Add(benchmark.ObjectClonerDeepClone());
            }
            Console.WriteLine(clones.Count);
#endif
        }
    }
}