using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCloner.Internal
{
    internal class DeepCloneExpressionBuilder<T>
    {
        private static readonly Type _typeOfT = typeof(T);
        
        private readonly ParameterExpression _originalParameter = Expression.Parameter(_typeOfT, "original");
        private readonly ParameterExpression _dictionaryParameter = Expression.Parameter(typeof(Dictionary<object,object>), "dict");
        private readonly ParameterExpression _cloneVariable = Expression.Variable(_typeOfT, "clone");
        private readonly LabelTarget _returnTarget = Expression.Label(_typeOfT);

        public Expression<Func<T, Dictionary<object, object>, T>> Build()
        {
            // Here is the method we need to create:
            // Lines prefixed by # only apply if T is a reference type.
            //
            // (T original, Dictionary<object, object> dict) => {
            //#     if(original == null)
            //#         return null;
            //
            //      //Special treatment for arrays
            //#
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
            
            List<Expression> expressions = new List<Expression>(10);
            
            if (!_typeOfT.IsValueType)
            {
                expressions.Add(CreateReturnIfNullExpression());
                
                expressions.Add(CreateReturnIfInDictionaryExpression());
            }

            if (_typeOfT.IsArray)
            {
                expressions.Add(CreateArrayCloneExpression());
            }
            else
            {
                expressions.Add(CreateMemberwiseCloneExpression());
            
                if (!_typeOfT.IsValueType)
                {
                    expressions.Add(CreateAddToDictionaryExpression());
                }
            
                expressions.AddRange(CreateFieldCopyExpressions());
            }
            
            
            
            
            expressions.Add(Expression.Label(_returnTarget, _cloneVariable));


            var functionBlock = Expression.Block(new[] {_cloneVariable}, expressions);

            return Expression.Lambda<Func<T, Dictionary<object, object>, T>>(functionBlock, _originalParameter, _dictionaryParameter);
        }
        
        private ConditionalExpression CreateReturnIfNullExpression()
        {
            // if(original == null)
            //         return null;
            
            return Expression.IfThen(
                Expression.ReferenceEqual(_originalParameter, Expression.Constant(null)),
                Expression.Return(_returnTarget, Expression.Constant(null, typeof(T)))
            );
        }
        private Expression CreateReturnIfInDictionaryExpression()
        {
            //    if(dict.TryGetValue(original, out object existingClone))
            //        return existingClone;
            
            var tryGetValueMethod = typeof(Dictionary<object, object>).GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(tryGetValueMethod != null);
            
            var outTVariable = Expression.Variable(typeof(object));
            return Expression.Block(
                new[] { outTVariable },
                Expression.IfThen(
                        Expression.IsTrue(
                                Expression.Call(
                                    _dictionaryParameter,
                                    tryGetValueMethod,
                                    _originalParameter,
                                    outTVariable
                                    )),
                        Expression.Return(_returnTarget, Expression.Convert(outTVariable, _typeOfT))
                    )
            );
        }

        private Expression CreateArrayCloneExpression()
        {
            // Arrays need special treatment. We generate code like this:
            // var length = original.Length;
            // TItem[] clone = new TItem[length]();
            // for(int i = 0; i < length; i++){
            //     clone[i] = DeepCloneInternal<TItem>.FieldCloner.Invoke(original[i], dict);
            // }

            Type itemType = _typeOfT.GetElementType();
            Debug.Assert(itemType != null);
            FieldInfo itemCloner = typeof(DeepCloneInternal<>).MakeGenericType(itemType).GetField("DeepCloner", BindingFlags.Static | BindingFlags.Public);
            Debug.Assert(itemCloner != null);
            MethodInfo invokeMethod = itemCloner.FieldType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            Debug.Assert(invokeMethod != null);

            
            ParameterExpression lengthVariable = Expression.Variable(typeof(int));
            ParameterExpression indexVariable = Expression.Variable(typeof(int));
            LabelTarget breakTarget = Expression.Label();

            return Expression.Block(
                new[] { lengthVariable, indexVariable },
                Expression.Assign(
                    lengthVariable,
                    Expression.ArrayLength(_originalParameter)
                ),
                Expression.Assign(
                    _cloneVariable,
                    Expression.NewArrayBounds(itemType, lengthVariable)
                ),
                Expression.Assign(
                    indexVariable,
                    Expression.Constant(0))
                ,
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                                Expression.GreaterThanOrEqual(indexVariable, lengthVariable),
                                Expression.Break(breakTarget)
                            ),
                        Expression.Assign(
                            Expression.ArrayAccess(_cloneVariable, indexVariable),
                            Expression.Call(
                                Expression.Field(null, itemCloner),
                                invokeMethod,
                                Expression.ArrayIndex(_originalParameter, indexVariable),
                                _dictionaryParameter
                                )
                            ),
                        Expression.PostIncrementAssign(indexVariable)
                        ),
                    breakTarget
                ),
                Expression.Return(_returnTarget, _cloneVariable));
        }
        
        private Expression CreateMemberwiseCloneExpression()
        {
            MethodInfo cloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(cloneMethod != null);
            
            var cloneExpression = Expression.Assign(
                _cloneVariable,
                Expression.Convert(
                    Expression.Call(
                        _originalParameter,
                        cloneMethod
                    ), 
                    _typeOfT
                ));

            return cloneExpression;
        }
        
        
        private Expression CreateAddToDictionaryExpression()
        {
            MethodInfo addMethod = typeof(Dictionary<object,object>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(addMethod != null);
            return Expression.Call(
                _dictionaryParameter,
                addMethod,
                _originalParameter,
                _cloneVariable
            );
        }
        
        private IEnumerable<Expression> CreateFieldCopyExpressions()
        {
            var fields = _typeOfT.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            
            foreach (FieldInfo field in fields)
            {
                FieldInfo fieldCloner = typeof(DeepCloneInternal<>).MakeGenericType(field.FieldType).GetField("DeepCloner", BindingFlags.Static | BindingFlags.Public);
                Debug.Assert(fieldCloner != null);
                MethodInfo invokeMethod = fieldCloner.FieldType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
                Debug.Assert(invokeMethod != null);

                yield return Expression.Assign(
                    Expression.Field(_cloneVariable, field),
                    Expression.Call(
                        Expression.Field(null, fieldCloner),
                        invokeMethod,
                        Expression.Field(_originalParameter, field),
                        _dictionaryParameter
                    )
                );
            }
        }
    }
}