using System;
using System.Collections.Generic;
using System.Reflection;

using Xunit;

namespace ObjectCloner.Tests.ShallowClone
{
    public class ClassCloneTests
    {
        [Fact]
        public void HandlesNull()
        {
            TestClass clone = ObjectCloner<TestClass>.ShallowClone(null);
            
            Assert.Null(clone);
        }

        [Fact]
        public void CopiesTestClass()
        {
            TestClass original = new TestClass
            {
                PrimitiveProperty = 42,
                StructProperty = new DateTime(1997,11,6),
                ListProperty = new List<int> {2,3,4},
                ArrayProperty = new []{2,3,4}
            };

            TestClass clone = ObjectCloner<TestClass>.ShallowClone(original);
            
            Assert.NotSame(original, clone);
            
            Assert.Equal(original.PrimitiveProperty, clone.PrimitiveProperty);
            Assert.Equal(original.StructProperty, clone.StructProperty);
            Assert.Same(original.ListProperty, clone.ListProperty);
            Assert.Same(original.ArrayProperty, clone.ArrayProperty);
        }

        public void CopiesPrivateField()
        {
            TestClassPrivateField original = new TestClassPrivateField(42);

            TestClassPrivateField clone = ObjectCloner<TestClassPrivateField>.ShallowClone(original);
            
            Assert.Equal(42, (int)typeof(TestClassPrivateField).GetField("_privateField", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(clone));
        }
        

        private class TestClass
        {
            public int PrimitiveProperty { get; set; }

            public DateTime StructProperty { get; set; }

            public List<int> ListProperty { get; set; }

            public int[] ArrayProperty { get; set; }
        }

        private class TestClassPrivateField
        {
            private int _privateField;

            public TestClassPrivateField(int privateField)
            {
                _privateField = privateField;
            }
        }
    }
}