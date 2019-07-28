using System;

using Xunit;

namespace ObjectCloner.Tests.ShallowClone
{
    public class PrimitiveCloneTests
    {
        [Fact]
        public void HandlesNull()
        {
            string clone = ObjectCloner<string>.ShallowClone(null);
            
            Assert.Null(clone);
        }
        
        [Fact]
        public void ClonesInt()
        {
            int originalMeaningOfLife = 42;

            int newMeaningOfLife = ObjectCloner<int>.ShallowClone(originalMeaningOfLife);
            
            Assert.Equal(originalMeaningOfLife, newMeaningOfLife);
        }
        
        
        [Fact]
        public void ClonesString()
        {
            string helloWorldOriginal = "Hello world";

            string newHelloWorld = ObjectCloner<string>.ShallowClone(helloWorldOriginal);
            
            // Here we want actual reference equality. A string is immutable, there would be no point in creating a new instance.
            // And because of Interning we might not get a new one even if we wanted to. (There has to be a way around that but why.)
            Assert.Same(helloWorldOriginal, newHelloWorld);
        }
    }
}