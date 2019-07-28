using System;

using Xunit;

namespace ObjectCloner.Tests.ShallowClone
{
    public class ArrayCloneTests
    {
        [Fact]
        public void CopiesArraysOfPrimitives()
        {
            int[] originalArray = { 3, 42, 123 };

            int[] newArray = ObjectCloner<int[]>.ShallowClone(originalArray);
            
            Assert.NotSame(originalArray, newArray);
            Assert.Equal(originalArray, newArray);
        }
        
        [Fact]
        public void CopiesArraysOfStructs()
        {
            DateTime[] originalArray = { new DateTime(1997, 11, 6),  new DateTime(2019, 7, 28) };

            DateTime[] newArray = ObjectCloner<DateTime[]>.ShallowClone(originalArray);
            
            Assert.NotSame(originalArray, newArray);
            Assert.Equal(originalArray, newArray);
        }

        [Fact]
        public void CopiesArraysOfClasses()
        {
            TestClass[] originalArray = { new TestClass(), new TestClass(), new TestClass(),  };

            TestClass[] newArray = ObjectCloner<TestClass[]>.ShallowClone(originalArray);
            
            Assert.NotSame(originalArray, newArray);
            Assert.Equal(originalArray, newArray);
        }

        private class TestClass
        {
        }
    }
}