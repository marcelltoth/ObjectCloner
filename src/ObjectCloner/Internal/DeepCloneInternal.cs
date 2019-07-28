using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        ///     The <see cref="IDictionary{TKey,TValue}"/> argument is used for handling circular references and multiple references to the same object.
        /// </remarks>
        public static Func<T, IDictionary<object, object>, T> DeepCloner { get; }

        static DeepCloneInternal()
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                DeepCloner = Identity;
                return;
            }

            DeepCloner = CreateDeepCloneMethod();
        }

        private static Func<T,IDictionary<object,object>,T> CreateDeepCloneMethod()
        {
            // Here is the method we need to create:
            // Lines prefixed by # only apply if T is a reference type.
            //
            // (T original, IDictionary<object, object> dict) => {
            //     if(original == null)
            //         return null;
            //
            //#    if(dict.TryGetValue(original, out object existingClone))
            //#        return existingClone;
            //
            //     var clone = (T)input.MemberwiseClone();
            //     dict.Add(original, clone);
            //     
            //     // Code to recursively clone fields that are not deeply readonly (primitives, strings, etc. should not be copied)
            //
            //     return clone;
            // }
            
            
            ParameterExpression originalParameter = Expression.Parameter(typeof(T));
            ParameterExpression dictionaryParameter = Expression.Parameter(typeof(IDictionary<object,object>));
            LabelTarget returnTarget = Expression.Label(typeof(T));

            
            Expression returnIfNullExpression = CreateReturnIfNullExpression(originalParameter, returnTarget);

            var functionBlock = Expression.Block(
                returnIfNullExpression,
                Expression.Label(returnTarget, Expression.Constant(null, typeof(T)))
            );
            
            var functionExpression = Expression.Lambda<Func<T, IDictionary<object, object>, T>>(functionBlock, originalParameter, dictionaryParameter);
            return functionExpression.Compile();
        }


        private static ConditionalExpression CreateReturnIfNullExpression(ParameterExpression originalParameter, LabelTarget returnTarget)
        {
            return Expression.IfThenElse(
                Expression.ReferenceEqual(originalParameter, Expression.Constant(null)),
                Expression.Constant(null),
                Expression.Return(returnTarget, Expression.Constant(null, typeof(T)))
            );
        }
        private static Expression CreateReturnIfInDictionaryExpression(ParameterExpression originalParameter, ParameterExpression dictionaryParameter)
        {
            return Expression.Throw(Expression.New(typeof(NotImplementedException)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Identity(T input, IDictionary<object, object> _) => input;
    }
}