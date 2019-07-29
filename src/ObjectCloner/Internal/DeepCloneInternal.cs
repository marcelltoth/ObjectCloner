using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ObjectCloner.Internal
{
    internal static class DeepCloneInternal<T>
    {
        /// <summary>
        ///     Function for the internal implementation of deep cloning.
        ///     First argument is the object to be cloned, second is a map in which already-cloned objects are stored. Call with an empty Dictionary initially.
        /// </summary>
        /// <remarks>
        ///     The <see cref="Dictionary{TKey,TValue}"/> argument is used for handling circular references and multiple references to the same object.
        /// </remarks>
        public static readonly Func<T, Dictionary<object, object>, T> DeepCloner;

        static DeepCloneInternal()
        {
            if (TypeHelper<T>.CanSkipDeepClone)
            {
                DeepCloner = Identity;
                return;
            }

            var builder = new DeepCloneExpressionBuilder<T>();
            DeepCloner = builder.Build().Compile();
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Identity(T input, Dictionary<object, object> _) => input;
    }
}