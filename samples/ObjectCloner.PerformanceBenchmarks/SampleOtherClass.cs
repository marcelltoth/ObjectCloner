using System;

namespace ObjectCloner.PerformanceBenchmarks
{
    [Serializable] // Not needed for ObjectCloner itself
    public class SampleOtherClass
    {
        public string SomeDeepProp { get; set; }
    }
}