using System;
using System.Collections.Generic;

using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class ArrayCloneTests
    {
        [Fact]
        public void CopiesArraysOfPrimitives()
        {
            int[] originalArray = { 3, 42, 123 };

            int[] newArray = ObjectCloner<int[]>.DeepClone(originalArray);
            
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
            TestClass[] originalArray = { new TestClass(2), new TestClass(3), new TestClass(5),  };

            TestClass[] newArray = ObjectCloner<TestClass[]>.ShallowClone(originalArray);
            
            // Assert same values but different objects
            Assert.NotSame(originalArray, newArray);
            Assert.NotEqual(originalArray, newArray, EqualityComparer<object>.Default);
            Assert.Equal(originalArray, newArray, EqualityComparer<TestClass>.Default);
        }

        private class TestClass
            : IEquatable<TestClass>
        {
            public TestClass(int someIntProp)
            {
                SomeIntProp = someIntProp;
            }

            public int SomeIntProp { get; }


            #region Equality

            public bool Equals(TestClass other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return SomeIntProp == other.SomeIntProp;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((TestClass)obj);
            }

            public override int GetHashCode()
            {
                return SomeIntProp;
            }

            #endregion
        }
    }
}