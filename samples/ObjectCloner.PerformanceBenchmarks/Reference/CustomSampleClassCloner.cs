namespace ObjectCloner.PerformanceBenchmarks.Reference
{
    public static class CustomSampleClassCloner
    {
        public static SampleClass DeepClone(SampleClass input)
        {
            if (input == null)
                return null;
            return new SampleClass
            {
                ArrayProp = (int[])input.ArrayProp?.Clone(),
                IntegerProp = input.IntegerProp,
                StringProp = input.StringProp,
                SampleDeepClass = input.SampleDeepClass == null ? null : new SampleOtherClass
                {
                    SomeDeepProp = input.SampleDeepClass.SomeDeepProp
                }
            };
        }
    }
}