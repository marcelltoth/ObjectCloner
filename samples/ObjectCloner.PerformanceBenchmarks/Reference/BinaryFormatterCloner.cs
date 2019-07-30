using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectCloner.PerformanceBenchmarks.Reference
{
    public static class BinaryFormatterCloner
    {
        public static object DeepClone(object input)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, input);
                ms.Position = 0;

                return formatter.Deserialize(ms);
            }
        }
    }
}