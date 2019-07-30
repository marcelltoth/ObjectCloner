using System;

namespace ObjectCloner.PerformanceBenchmarks
{
    [Serializable] // Not needed for ObjectCloner itself
    public class SampleClass
    {
        public int IntegerProp { get; set; }

        public string StringProp { get; set; }

        public int[] ArrayProp { get; set; }

        public SampleOtherClass SampleDeepClass { get; set; }

        public override string ToString()
        {
            return "Complex class";
        }
    }
}