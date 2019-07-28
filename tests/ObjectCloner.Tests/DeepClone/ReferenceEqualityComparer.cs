using System;
using System.Collections.Generic;

namespace ObjectCloner.Tests.DeepClone
{
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
        
        public static ReferenceEqualityComparer<T> Default { get; } = new ReferenceEqualityComparer<T>();
    }
}