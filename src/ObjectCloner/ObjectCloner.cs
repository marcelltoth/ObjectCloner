using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using ObjectCloner.Internal;

namespace ObjectCloner
{
    /// <summary>
    ///     Provides static helper methods that clone an object of type <typeparamref name="T"/>, either shallowly or deeply.
    /// </summary>
    /// <typeparam name="T">The type of object to clone.</typeparam>
    public static class ObjectCloner<T>
    {
        /// <summary>
        ///     Creates a shallow copy of <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The object to copy. Can be null.</param>
        /// <returns>Returns a new object of type <typeparamref name="T"/>, with all fields set to the same value as in <paramref name="original"/>.</returns>
        /// <remarks>
        ///     <para>
        ///         There are no generic constraints on <typeparamref name="T"/>, however for obvious reasons this method is a noop for value types.
        ///     </para>
        ///     <para>
        ///         If <typeparamref name="T"/> is a value type (including primitives) or <code>string</code>, this method will simply return the input.
        ///     </para>
        ///     <para>
        ///         If <typeparamref name="T"/> is a reference type (including primitives) this method will create a memberwise clone.
        ///         There is no need for <typeparamref name="T"/> to contain a parameterless constructor, no constructor will be called.
        ///         For this reason be cautious with cloning types requiring special initialization / cleanup (e.g. ones implementing <see cref="IDisposable"/>).
        ///     </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ShallowClone(T original)
        {
            return ShallowCopyInternal<T>.ShallowCopier(original);
        }

        /// <summary>
        ///     Performs a deep clone on <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The object to clone, or null.</param>
        /// <returns>A new object of type <typeparamref name="T"/>, with all fields cloned deeply.</returns>
        /// <remarks>
        ///     <para>
        ///         There are no generic constraints on <typeparamref name="T"/>, however for obvious reasons this method is a noop for value types.
        ///     </para>
        ///     <para>
        ///         If <typeparamref name="T"/> is a primitive or <code>string</code>, or <paramref name="original"/> is <code>null</code>, this method will simply return the input.
        ///     </para>
        ///     <para>
        ///         If <typeparamref name="T"/> is a reference type or a non-primitive value type, this method will create a deep clone, recursively applying this function to every field.
        ///         There is no need for <typeparamref name="T"/> to contain a parameterless constructor, no constructor will be called.
        ///         For this reason be cautious with cloning types requiring special initialization / cleanup (e.g. ones implementing <see cref="IDisposable"/>, or ones interfacing with unmanaged code).
        ///     </para>
        ///     <para>
        ///         Reference equality inside the tree will be conserved.
        ///         This means that if a single object is referenced from multiple fields of <paramref name="original"/>, all those fields will point to the same instance in the clone as well.
        ///         (Albeit to a deeply different instance from the original.)
        ///         This includes handling of circular dependencies as well.
        ///     </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeepClone(T original)
        {
            return DeepCloneInternal<T>.DeepCloner(original, new Dictionary<object, object>());
        }
    }
}