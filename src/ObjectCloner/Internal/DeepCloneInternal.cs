using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
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
        public static Func<T, Dictionary<object, object>, T> DeepCloner { get; }

        static DeepCloneInternal()
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                DeepCloner = Identity;
                return;
            }

            DeepCloner = CreateDeepCloneMethod();
        }

        private static Func<T,Dictionary<object,object>,T> CreateDeepCloneMethod()
        {
            // Here is the method we need to create:
            // Lines prefixed by # only apply if T is a reference type.
            //
            // (T original, Dictionary<object, object> dict) => {
            //     if(original == null)
            //         return null;
            //
            //#    if(dict.TryGetValue(original, out object existingClone))
            //#        return existingClone;
            //
            //     var clone = (T)input.MemberwiseClone();
            //#     dict.Add(original, clone);
            //     
            //     // Code to recursively clone fields that are not deeply readonly (primitives, strings, etc. should not be copied)
            //
            //     return clone;
            // }


            Type typeOfT = typeof(T);
            ParameterExpression originalParameter = Expression.Parameter(typeOfT);
            ParameterExpression dictionaryParameter = Expression.Parameter(typeof(Dictionary<object,object>));
            ParameterExpression cloneVariable = Expression.Variable(typeOfT);
            LabelTarget returnTarget = Expression.Label(typeOfT);

            List<Expression> expressions = new List<Expression>(10);
            
            expressions.Add(CreateReturnIfNullExpression(originalParameter, returnTarget));

            if (!typeOfT.IsValueType)
            {
                expressions.Add(CreateReturnIfInDictionaryExpression(originalParameter, dictionaryParameter, returnTarget));
            }
            
            
            expressions.Add(CreateMemberwiseCloneExpression(originalParameter, cloneVariable));
            
            if (!typeOfT.IsValueType)
            {
                expressions.Add(CreateAddToDictionaryExpression(originalParameter, dictionaryParameter, cloneVariable));
            }
            
            expressions.Add(Expression.Label(returnTarget, cloneVariable));


            var functionBlock = Expression.Block(new[] {cloneVariable}, expressions);
            
            var functionExpression = Expression.Lambda<Func<T, Dictionary<object, object>, T>>(functionBlock, originalParameter, dictionaryParameter);
            return functionExpression.Compile();
        }



        private static ConditionalExpression CreateReturnIfNullExpression(ParameterExpression originalParameter, LabelTarget returnTarget)
        {
            // if(original == null)
            //         return null;
            
            return Expression.IfThen(
                Expression.ReferenceEqual(originalParameter, Expression.Constant(null)),
                Expression.Return(returnTarget, Expression.Constant(null, typeof(T)))
            );
        }
        private static Expression CreateReturnIfInDictionaryExpression(ParameterExpression originalParameter, ParameterExpression dictionaryParameter, LabelTarget returnTarget)
        {
            //    if(dict.TryGetValue(original, out object existingClone))
            //        return existingClone;
            
            var tryGetValueMethod = typeof(Dictionary<object, object>).GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(tryGetValueMethod != null);
            
            var outTVariable = Expression.Variable(typeof(T));
            return Expression.Block(
                new[] { outTVariable },
                Expression.IfThen(
                        Expression.IsTrue(
                                Expression.Call(
                                    dictionaryParameter,
                                    tryGetValueMethod,
                                    originalParameter,
                                    outTVariable
                                    )),
                        Expression.Return(returnTarget, outTVariable)
                    )
            );
        }
        
        private static Expression CreateMemberwiseCloneExpression(ParameterExpression originalParameter, ParameterExpression cloneVariable)
        {
            MethodInfo cloneMethod = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(cloneMethod != null);
            
            var cloneExpression = Expression.Assign(
                cloneVariable,
                Expression.Convert(
                    Expression.Call(
                        originalParameter,
                        cloneMethod
                    ), 
                    typeof(T)
                ));

            return cloneExpression;
        }
        
        
        private static Expression CreateAddToDictionaryExpression(ParameterExpression originalParameter, ParameterExpression dictionaryParameter, ParameterExpression cloneVariable)
        {
            
            MethodInfo addMethod = typeof(Dictionary<object,object>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(addMethod != null);
            return Expression.Call(
                dictionaryParameter,
                addMethod,
                originalParameter,
                cloneVariable
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Identity(T input, Dictionary<object, object> _) => input;
    }
}