using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCloner.Internal
{
    internal class DeepCloneExpressionBuilder
    {
        private static readonly Type _typeOfObject = typeof(object);
        private readonly Type _typeOfT;
        
        private readonly ParameterExpression _originalParameter;
        private readonly ParameterExpression _originalVariable;
        private readonly ParameterExpression _dictionaryParameter;
        private readonly ParameterExpression _cloneVariable;
        private readonly LabelTarget _returnTarget;
        
        
        private readonly MethodInfo itemClonerGetter = typeof(DeepCloneInternal).GetMethod("GetDeepCloner", BindingFlags.Static | BindingFlags.Public);
        private readonly MethodInfo invokeMethod = typeof(DeepCloner).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
        private readonly MethodInfo getTypeMethod = _typeOfObject.GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance);
        private readonly MethodInfo arrayCloneMethod = typeof(Array).GetMethod("Clone", BindingFlags.Public | BindingFlags.Instance);

        public DeepCloneExpressionBuilder(Type typeOfT)
        {
            _typeOfT = typeOfT;
            _dictionaryParameter = Expression.Parameter(typeof(Dictionary<object,object>), "dict");
            _returnTarget = Expression.Label(_typeOfObject);
            _cloneVariable = Expression.Variable(_typeOfT, "clone");
            _originalParameter = Expression.Parameter(_typeOfObject, "original");
            _originalVariable = Expression.Parameter(_typeOfT, "originalCasted");
            
            
            Debug.Assert(itemClonerGetter != null);
            Debug.Assert(invokeMethod != null);
            Debug.Assert(getTypeMethod != null);
        }


        public Expression<DeepCloner> Build()
        {
            // Here is the method we need to create:
            // Lines prefixed by # only apply if T is a reference type.
            //
            // (objct original, Dictionary<object, object> dict) => {
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
            
            expressions.Add(Expression.Assign(
                    _originalVariable,
                    Expression.Convert(
                            _originalParameter,
                            _typeOfT
                        )
                ));
            
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
            
            
            
            
            expressions.Add(Expression.Label(_returnTarget, Expression.Convert(_cloneVariable, _typeOfObject)));


            var functionBlock = Expression.Block(new[] {_cloneVariable, _originalVariable}, expressions);

            return Expression.Lambda<DeepCloner>(functionBlock, _originalParameter, _dictionaryParameter);
        }
        
        private ConditionalExpression CreateReturnIfNullExpression()
        {
            // if(original == null)
            //         return null;
            
            return Expression.IfThen(
                Expression.ReferenceEqual(_originalVariable, Expression.Constant(null)),
                Expression.Return(_returnTarget, Expression.Constant(null, _typeOfObject))
            );
        }
        private Expression CreateReturnIfInDictionaryExpression()
        {
            //    if(dict.TryGetValue(original, out object existingClone))
            //        return existingClone;
            
            var tryGetValueMethod = typeof(Dictionary<object, object>).GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(tryGetValueMethod != null);
            
            var outTVariable = Expression.Variable(_typeOfObject);
            return Expression.Block(
                new[] { outTVariable },
                Expression.IfThen(
                        Expression.IsTrue(
                                Expression.Call(
                                    _dictionaryParameter,
                                    tryGetValueMethod,
                                    _originalVariable,
                                    outTVariable
                                    )),
                        Expression.Return(_returnTarget, outTVariable)
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

            if (TypeHelper.CanSkipDeepClone(itemType))
            {
                return Expression.Assign(
                    _cloneVariable,
                    Expression.Convert(
                            Expression.Call(_originalVariable, arrayCloneMethod),
                            _typeOfT
                        )
                );
            }
            
            ParameterExpression lengthVariable = Expression.Variable(typeof(int));
            ParameterExpression indexVariable = Expression.Variable(typeof(int));
            LabelTarget breakTarget = Expression.Label();

            return Expression.Block(
                new[] { lengthVariable, indexVariable },
                Expression.Assign(
                    lengthVariable,
                    Expression.ArrayLength(_originalVariable)
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
                            Expression.Convert(
                                CreateRecursiveCallExpression(Expression.ArrayAccess(_originalVariable, indexVariable)),
                                itemType
                                )
                            ),
                        Expression.PostIncrementAssign(indexVariable)
                        ),
                    breakTarget
                ),
                Expression.Return(_returnTarget, Expression.Convert(_cloneVariable, _typeOfObject)));
        }

        private Expression CreateRecursiveCallExpression(Expression objectToCopy)
        {
            // Create code like so:
            // __INPUT_XPR__ != null ? DeepCloneInternal.GetDeepCloner((__INPUT_XPR__).GetType())(__INPUT_XPR__, dict) : null
            // where __INPUT_XPR__ is the input expression (objectToCopy)
            return Expression.Condition(
                    Expression.ReferenceEqual(Expression.Convert(objectToCopy, _typeOfObject), Expression.Constant(null)),
                    Expression.Convert(Expression.Constant(null), _typeOfObject),
                    Expression.Call(
                        Expression.Call(null, itemClonerGetter, Expression.Call(
                            objectToCopy,
                            getTypeMethod
                        )),
                        invokeMethod,
                        Expression.Convert(objectToCopy, _typeOfObject),
                        _dictionaryParameter
                    )
                );
        }

        private Expression CreateMemberwiseCloneExpression()
        {
            MethodInfo cloneMethod = _typeOfObject.GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(cloneMethod != null);
            
            var cloneExpression = Expression.Assign(
                _cloneVariable,
                Expression.Convert(
                    Expression.Call(
                        _originalVariable,
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
                _originalVariable,
                _cloneVariable
            );
        }
        
        private IEnumerable<Expression> CreateFieldCopyExpressions()
        {
            IEnumerable<FieldInfo> fields = _typeOfT.GetAllFieldsDeep();
            
            foreach (FieldInfo field in fields)
            {
                // Skip fields that are deeply immutable
                if (TypeHelper.CanSkipDeepClone(field.FieldType))
                {
                    continue;
                }


                MemberExpression cloneFieldExpression = Expression.Field(_cloneVariable, field);

                if (!field.IsInitOnly)
                {
                    // Read-write fields are easy to set. Generate code like:
                    // clone.field = DeepCloneInternal<TypeOfField>.DeepCloner(original.field, dict);
                    
                    yield return Expression.Assign(
                        cloneFieldExpression,
                        Expression.Convert(
                            CreateRecursiveCallExpression(Expression.Field(_originalVariable, field)),
                            field.FieldType
                            )
                    );
                }
                else
                {
                    // Readonly fields can be written using reflection. (Although it is an implementation detail: https://stackoverflow.com/questions/934930/can-i-change-a-private-readonly-field-in-c-sharp-using-reflection#comment743456_934944)
                    // Code: fieldInfoConstant.SetValue(clone.field, DeepCloneInternal<TypeOfField>.DeepCloner(original.field, dict));
                    
                    MethodInfo setValueMethod = typeof(FieldInfo).GetMethod("SetValue", new[] { _typeOfObject, _typeOfObject });
                    Debug.Assert(setValueMethod != null);

                    yield return Expression.Call(
                        Expression.Constant(field),
                        setValueMethod,
                        Expression.Convert(_cloneVariable, _typeOfObject),
                        CreateRecursiveCallExpression(Expression.Field(_originalVariable, field)));
                }
                
                
            }
        }
    }
}