using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCloner.Internal
{
    internal static class ShallowCopyInternal<T>
    {
        public static readonly Func<T, T> ShallowCopier;

        static ShallowCopyInternal()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                ShallowCopier = Identity;
                return;
            }

            ShallowCopier = CreateShallowCopyExpressionLamda().Compile();
        }

        private static Expression<Func<T, T>> CreateShallowCopyExpressionLamda()
        {
            // Create a lambda like so:
            // (T input) => (T)input.Membm
            
            
            var cloneMethod = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(cloneMethod != null);
            
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "input");
            var cloneExpression = Expression.Lambda<Func<T, T>>(
                Expression.Convert(
                    Expression.Call(
                        parameterExpression,
                        cloneMethod
                    ),
                    typeof(T)
                ), 
                parameterExpression);

            return cloneExpression;
        }

        private static T Identity(T input) => input;
    }
}