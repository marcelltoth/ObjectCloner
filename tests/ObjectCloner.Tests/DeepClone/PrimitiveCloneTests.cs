using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class PrimitiveCloneTests
    {
        [Fact]
        public void HandlesNull()
        {
            string clone = ObjectCloner.DeepClone<string>(null);
            
            Assert.Null(clone);
        }

        [Fact]
        public void ClonesInt()
        {
            int original = 42;

            int clone = ObjectCloner.DeepClone(original);
            
            Assert.Equal(original, clone);
        }

        [Fact]
        public void ClonesStringWithoutAllocation()
        {
            string original = "Hello world";

            string clone = ObjectCloner.DeepClone(original);
            
            Assert.Same(original, clone);
        }
    }
}