using System;

using Xunit;

namespace ObjectCloner.Tests.ShallowClone
{
    public class StructCloneTests
    {
        [Fact]
        public void ClonesDateTime()
        {
            DateTime myBirthday = new DateTime(1997, 11, 6); // no, I'm not a narcist at all

            DateTime newBirthday = ObjectCloner.ShallowClone(myBirthday);
            
            // These are value types, a reference comparison does not make sense
            Assert.Equal(newBirthday, myBirthday);
        }
    }
}