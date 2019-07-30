using Newtonsoft.Json;

namespace ObjectCloner.PerformanceBenchmarks.Reference
{
    public static class NewtonsoftJsonCloner
    {
        public static T DeepClone<T>(T original)
        {
            var serialized = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}