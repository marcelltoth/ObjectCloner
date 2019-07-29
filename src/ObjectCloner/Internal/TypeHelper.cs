using System;
using System.Collections.Concurrent;

namespace ObjectCloner.Internal
{
    /// <summary>
    ///     Contains optimized helper methods about types.
    /// </summary>
    internal static class TypeHelper
    {
        private static readonly ConcurrentDictionary<Type, bool> _canSkipDeepCloneMap = new ConcurrentDictionary<Type, bool>();
        
        /// <summary>
        ///     True if we can safely skip deep cloning of an object of type <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        ///     True in the case of deeply immutable objects.
        /// </remarks>
        public static bool CanSkipDeepClone(Type type)
        {
            return _canSkipDeepCloneMap.GetOrAdd(type, t => t.IsPrimitive || t == typeof(string) || t == typeof(object));
        }
    }
}