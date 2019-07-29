using System;

using Xunit;

namespace ObjectCloner.Tests.DeepClone
{
    public class FieldCloneTests
    {

        [Fact]
        public void DeepClonesPrivateField()
        {
            ClassWithAPrivateField original = new ClassWithAPrivateField(42);
            ClassWithAPrivateField clone = ObjectCloner.DeepClone(original);
            
            Assert.NotSame(original.GetPrivateField(), clone.GetPrivateField());
            Assert.Equal(original.GetPrivateField(), clone.GetPrivateField());
        }
        
        [Fact]
        public void DeepClonesReadonlyField()
        {
            ClassWithAReadonlyField original = new ClassWithAReadonlyField(42);
            ClassWithAReadonlyField clone = ObjectCloner.DeepClone(original);
            
            Assert.NotSame(original.GetPrivateField(), clone.GetPrivateField());
            Assert.Equal(original.GetPrivateField(), clone.GetPrivateField());
        }
        

        private class ClassWithAPrivateField
        {
            public ClassWithAPrivateField(int someProp)
            {
                _privateField = new OtherClass(someProp);
            }
            
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            private OtherClass _privateField;

            public OtherClass GetPrivateField() => _privateField;
        }
        
        private class ClassWithAReadonlyField
        {
            public ClassWithAReadonlyField(int someProp)
            {
                _privateField = new OtherClass(someProp);
            }
            
            private readonly OtherClass _privateField;

            public OtherClass GetPrivateField() => _privateField;
        }

        public class OtherClass
            : IEquatable<OtherClass>
        {
            public OtherClass(int someProp)
            {
                SomeProp = someProp;
            }

            public int SomeProp { get; }


            #region Equality

            public bool Equals(OtherClass other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return SomeProp == other.SomeProp;
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

                return Equals((OtherClass)obj);
            }

            public override int GetHashCode()
            {
                return SomeProp;
            }

            #endregion
        }
    }
}