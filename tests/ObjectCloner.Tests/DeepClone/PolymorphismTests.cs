using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class PolymorphismTests
    {

        [Fact]
        public void DeepClonesByInterface()
        {
            TestClassTwo innerData = new TestClassTwo(42);
            TestClass original = new TestClass(innerData);

            TestClass clone = ObjectCloner.DeepClone(original);
            
            Assert.NotSame(clone.TestListPolymorphic, original.TestListPolymorphic);
            Assert.NotSame(clone.TestListPolymorphic.Last(), original.TestListPolymorphic.Last());
            Assert.Equal(clone.TestListPolymorphic.Last().SomeProp, original.TestListPolymorphic.Last().SomeProp);

        }
        

        private class TestClass
        {
            public TestClass(params TestClassTwo[] testData)
            {
                TestListPolymorphic = testData;
            }

            public IEnumerable<TestClassTwo> TestListPolymorphic { get; }
        }

        private class TestClassTwo
        {
            public TestClassTwo(int someProp)
            {
                SomeProp = someProp;
            }

            public int SomeProp { get; }
        }
    }
}