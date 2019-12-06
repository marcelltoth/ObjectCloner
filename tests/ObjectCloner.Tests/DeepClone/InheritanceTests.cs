using System.Collections.Generic;

using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class InheritanceTests
    {
        [Fact]
        public void ClonesPropertyOnBaseClass()
        {
            var input = new MainTestClass
            {
                ListClassesBase = new List<SecondClass>()
            };

            var clone = ObjectCloner.DeepClone(input);
            
            Assert.NotSame(input.ListClassesBase, clone.ListClassesBase);
        }
        
        private class SecondClass
        {
            public int Value { get; set; }
        }

        private class BaseClass
        {
            public IList<SecondClass> ListClassesBase { get; set; }
        }
         
        private class MainTestClass : BaseClass
        {
            public IList<SecondClass> ListClasses { get; set; }
        }
    }
}