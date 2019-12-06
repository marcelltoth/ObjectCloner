using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        /// <summary>
        ///     Returns all the fields that <paramref name="type"/> or any of its base classes declare.
        ///     Returns protected and private fields as well.
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFieldsDeep(this Type type)
        {
            // This shortcut is needed because of the recursive call below.
            if(type == typeof(object))
                return Enumerable.Empty<FieldInfo>();
            
            // Fetch the fields declared on the current type
            IEnumerable<FieldInfo> ownFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            
            // Recursively call this method with the parent type and append that to the list.
            return ownFields.Concat(GetAllFieldsDeep(type.BaseType));
        }
    }
}