using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using ObjectCloner.PerformanceBenchmarks.Reference;

namespace ObjectCloner.PerformanceBenchmarks
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, RPlotExporter]
    [SimpleJob(1,  5, 10)]
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

        [Benchmark(Baseline = true)]
        public object CloneViaCustomWrittenCode()
        {
            switch (Original)
            {
                case SampleClass sc:
                    return CustomSampleClassCloner.DeepClone(sc);
                default:
                    throw new NotImplementedException();
            }
        }


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
        
        [Benchmark]
        public object CloneViaNewtonsoftJson()
        {
            return NewtonsoftJsonCloner.DeepClone(Original);
        }
        
    }
}