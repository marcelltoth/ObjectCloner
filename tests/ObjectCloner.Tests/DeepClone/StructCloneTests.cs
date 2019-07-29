using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class StructCloneTests
    {

        [Fact]
        public void CopiesStructWithClassInside()
        {
            TestClass originalClass = new TestClass()
            {
                SomeValue = 42
            };
            TestStruct originalStruct = new TestStruct
            {
                PrimitiveProp = 42,
                ClassProp = originalClass
            };

            TestStruct resultStruct = ObjectCloner.DeepClone(originalStruct);
            
            Assert.Equal(originalStruct.PrimitiveProp, resultStruct.PrimitiveProp);
            Assert.NotSame(originalStruct.ClassProp, resultStruct.ClassProp);
            Assert.Equal(originalStruct.ClassProp.SomeValue, resultStruct.ClassProp.SomeValue);
        }

        [Fact]
        public void HandlesNullFields()
        {
            TestStruct originalStruct = new TestStruct();
            
            TestStruct result = ObjectCloner.DeepClone(originalStruct);
            
            Assert.Null(result.ClassProp);
        }
    

        private struct TestStruct
        {
            public int PrimitiveProp { get; set; }

            public TestClass ClassProp { get; set; }
        }

        private class TestClass
        {
            public int SomeValue { get; set; }
        }
    }
}