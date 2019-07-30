using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using ObjectCloner.PerformanceBenchmarks.Reference;

namespace ObjectCloner.PerformanceBenchmarks
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, RPlotExporter]
    [SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 10)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class PerformanceBenchmark
    {
        public static IEnumerable<object> CreateTestObjects()
        {
            yield return new SampleClass
            {
                ArrayProp = new []{2,4,5},
                IntegerProp = 42,
                SampleDeepClass = new SampleOtherClass
                {
                    SomeDeepProp = "Deep string"
                },
                StringProp = "Outer string prop"
            };
        }

        [ParamsSource(nameof(CreateTestObjects))]
        public object Original { get; set; }


        [Benchmark]
        public object CloneViaObjectCloner()
        {
            return ObjectCloner.DeepClone(Original);
        }

        [Benchmark]
        public object CloneViaBinaryFormatter()
        {
            return BinaryFormatterCloner.DeepClone(Original);
        }

        [Benchmark]
        public object CloneViaReflection()
        {
            return ReflectionCloner.Copy(Original);
        }
        
    }
}