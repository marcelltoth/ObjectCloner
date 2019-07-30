using System;

namespace ObjectCloner.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     <para>Performs a deep clone on <paramref name="original" />.</para>
        ///     <seealso cref="ObjectCloner.DeepClone{T}"/>
        /// </summary>
        /// <remarks>
        ///     Equivalent with <code>ObjectCloner.DeepClone(original)</code>.
        /// </remarks>
        /// <param name="original">The object to clone.</param>
        public static T DeepClone<T>(this T original) => ObjectCloner.DeepClone(original);
        
        /// <summary>
        ///     <para>Performs a shallow clone on <paramref name="original" />.</para>
        ///     <seealso cref="ObjectCloner.ShallowClone{T}"/>
        /// </summary>
        /// <remarks>
        ///     Equivalent with <code>ObjectCloner.ShallowClone(original)</code>.
        /// </remarks>
        /// <param name="original">The object to clone.</param>
        public static T ShallowClone<T>(this T original) => ObjectCloner.ShallowClone(original);
    }
}