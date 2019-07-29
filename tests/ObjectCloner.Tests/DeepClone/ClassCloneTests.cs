using System.Collections.Generic;

using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class ComplexClassCloneTests
    {
        [Fact]
        public void HandlesNull()
        {
            TestClassOne clone = ObjectCloner.DeepClone<TestClassOne>(null);
            
            Assert.Null(clone);
        }
        
        [Fact]
        public void HandlesComplexExample()
        {
            TestClassOne original = new TestClassOne()
            {
                ClassReference = null,
                NullString = null,
                StructProp = new TestStruct()
                {
                    IntegerOne = 4,
                    IntegerTwo = 2
                },
                StructArrayProp = new []
                {
                    new TestStruct(6, 9), 
                    new TestStruct(), 
                },
                DictionaryProp = new Dictionary<string, TestClassTwo>
                {
                    {"ASD", new TestClassTwo(42)},
                    {"BSD", new TestClassTwo(99)}
                }
            };

            TestClassOne clone = ObjectCloner.DeepClone(original);
            
            // TODO: break this into multiple smaller tests - the number of asssertions is a code smell itself
            Assert.NotSame(original, clone);
            Assert.Null(clone.NullString);
            Assert.Null(clone.ClassReference);
            Assert.Equal(4, clone.StructProp.IntegerOne);
            Assert.Equal(2, clone.StructProp.IntegerTwo);
            Assert.NotSame(original.StructArrayProp, clone.StructArrayProp);
            Assert.Equal((IEnumerable<TestStruct>)original.StructArrayProp, clone.StructArrayProp);
            Assert.NotSame(original.DictionaryProp, clone.DictionaryProp);
            Assert.NotSame(original.DictionaryProp["BSD"], clone.DictionaryProp["BSD"]);
            Assert.Equal(99, original.DictionaryProp["BSD"].PrimitiveProp);
        }

        [Fact]
        public void HandlesCircularReferences()
        {
            TestClassOne original = new TestClassOne();
            TestClassTwo originalTwo = new TestClassTwo(42);
            original.ClassReference = originalTwo;
            originalTwo.CircularReferenceProp = original;

            TestClassOne clone = ObjectCloner.DeepClone(original);
            Assert.NotSame(original, clone.ClassReference.CircularReferenceProp);
            Assert.Same(clone, clone.ClassReference.CircularReferenceProp);
        }

        [Fact]
        public void HandlesSameReferenceMultipleTimes()
        {
            TestClassTwo originalClass = new TestClassTwo(42);
            TestClassTwo[] originalArray = new[] { originalClass, originalClass };

            var clone = ObjectCloner.DeepClone(originalArray);
            
            Assert.Same(clone[0], clone[1]);
            Assert.NotSame(clone[0], originalClass);
        }

        private class TestClassOne
        {
            public string NullString { get; set; }

            public TestClassTwo ClassReference { get; set; }

            public TestStruct StructProp { get; set; }

            public TestStruct[] StructArrayProp { get; set; }
            
            public Dictionary<string, TestClassTwo> DictionaryProp { get; set; }
        }

        private class TestClassTwo
        {
            public TestClassTwo(int primitiveProp)
            {
                PrimitiveProp = primitiveProp;
            }

            public int PrimitiveProp { get; set; }

            public TestClassOne CircularReferenceProp { get; set; }
        }
        
        private struct TestStruct
        {
            public TestStruct(int integerOne, int integerTwo)
            {
                IntegerOne = integerOne;
                IntegerTwo = integerTwo;
            }

            public int IntegerOne { get; set; }

            public int IntegerTwo { get; set; }
        }
    }
}