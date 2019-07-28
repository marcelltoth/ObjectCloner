using System;

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
        public static T ShallowClone(T original)
        {
            throw new NotImplementedException();
        }
    }
}