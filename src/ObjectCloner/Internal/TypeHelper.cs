using System;

namespace ObjectCloner.Internal
{
    /// <summary>
    ///     Contains optimized helper methods about the type <typeparamref name="T"/>.
    /// </summary>
    internal static class TypeHelper<T>
    {
        /// <summary>
        ///     True if we can safely skip deep cloning of an object of type <typeparamref name="T"/>. 
        /// </summary>
        /// <remarks>
        ///     True in the case of deeply immutable objects.
        /// </remarks>
        public static bool CanSkipDeepClone { get; }

        static TypeHelper()
        {
            Type type = typeof(T);
            CanSkipDeepClone = CalculateCanSkipDeepClone(type);
        }

        private static bool CalculateCanSkipDeepClone(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }
    }
}