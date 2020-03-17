using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressiveAsserts
{
    public class TestCase<T> : IEnumerable<object>
    {
        private List<Expression<Func<T, bool>>> Assertions { get; } = new List<Expression<Func<T, bool>>>();
        
        private List<Expression<Func<T>>> ObjectAssertions { get; } = new List<Expression<Func<T>>>();
        
        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Expression<Func<T, bool>> assertion)
        {
            Assertions.Add(assertion);
        }

        public void Add(Expression<Func<T>> objectAssertion)
        {
            ObjectAssertions.Add(objectAssertion);
        }

        public void Verify(T result)
        {
            foreach (var assertion in Assertions)
            {
                var func = assertion.Compile();
                bool succeeded = false;
                try
                {
                    succeeded = func(result);
                }
                finally
                {
                    if (!succeeded)
                    {
                        throw GetErrorMessage(result, assertion);
                    }
                }
            }

            var objectEqualityBuilder = new ObjectEqualityBuilder();
            foreach (var assertion in ObjectAssertions)
            {
                var test = objectEqualityBuilder.AreEqual(result, assertion);
                if (test != null)
                {
                    throw test;
                }
            }
        }

        private Expression GetExpectedExpression(BinaryExpression e)
        {
            if (e.Left is ConstantExpression)
            {
                return e.Left;
            }

            return e.Right;
        }

        private Expression GetTestExpression(BinaryExpression e)
        {
            if (e.Left is ConstantExpression)
            {
                return e.Right;
            }

            return e.Left;
        }

        private VerificationException GetErrorMessage(T result, Expression<Func<T, bool>> assertion)
        {
            try
            {
                if (assertion.Body is BinaryExpression e)
                {
                    var expectedExpression = GetExpectedExpression(e);
                    var testExpression = GetTestExpression(e);
                    var names = string.Join(".", GetName(testExpression));
                    if (string.IsNullOrEmpty(names))
                    {
                        names = typeof(T).Name;
                    }

                    var actual = GetValue(result, testExpression);
                    var expected = GetValue(result, expectedExpression);

                    if (actual.Succeeded)
                    {
                        return new VerificationException($"Expected {names} to be '{expected}' but was '{actual}'")
                        {
                            ExpectedValue = expected.Value, ActualValue = actual.Value, Target = names
                        };
                    }

                    return new VerificationException($"Expected {names} to be '{expected}' but '{actual.NullPropertyName}' was null")
                    {
                        NullProperty = actual.NullPropertyName, Target = names
                    };
                }

                if (assertion.Body is MethodCallExpression mce)
                {
                    return GetMethodErrorMessage(result, assertion, mce);
                }

                if (assertion.Body is UnaryExpression u)
                {
                    if (u.NodeType != ExpressionType.Not)
                    {
                        throw new NotSupportedException("Expected not expression, got " + u.NodeType);
                    }

                    return GetMethodErrorMessageInverted(result, assertion, u.Operand);
                }

                return new VerificationException(new ExpressionStringBuilder().Build(assertion));
            }
            catch (Exception)
            {
                return new VerificationException(new ExpressionStringBuilder().Build(assertion));
            }
        }

        private VerificationException GetMethodErrorMessage(T result, Expression<Func<T, bool>> assertion, MethodCallExpression mce)
        {
            if (mce.Method.ReturnType != typeof(bool))
            {
                throw new NotSupportedException($"method verification must return bool, not {mce.Method.ReturnType}");
            }

            var actual = GetValue(result, (Expression)mce);
            var names = string.Join(".", GetName(mce));
            if (actual.Succeeded)
            {
                return new VerificationException($"Expected {names} to be true, but was false.") { Target = names };
            }

            if (actual.Enumerable != null)
            {
                var enumerableString = GetEnumerableString(actual.Enumerable);
                

                var message = $"Expected {names} to be true, but was false.";

                if (actual.Enumerable is string)
                {
                    return new VerificationException(message + $" String is <{enumerableString}>")
                    {
                        Target = names,
                        Enumerable = actual.Enumerable
                    };
                }
                return new VerificationException(message + $" Collection contains: {Environment.NewLine}{enumerableString}")
                {
                    Target = names,
                    Enumerable = actual.Enumerable
                };
            }

            return new VerificationException($"Tried to verify {names}, but {actual.NullPropertyName} was null")
            {
                Target = names
            };
        }

        private string GetEnumerableString(IEnumerable enumerable)
        {
            if (enumerable is string s)
            {
                return s;
            }
            
            var list = new List<string>();

            bool useToString = false;
            bool hasTestedToString = false;
            
            foreach (var x in enumerable)
            {
                if (x == null)
                {
                    list.Add("null");
                    continue;
                }
                
                if (!hasTestedToString)
                {
                    hasTestedToString = true;
                    var toStringMethod = x.GetType().GetMethod("ToString");
                    useToString = toStringMethod.DeclaringType != typeof(object);
                }
                {
                    if (useToString)
                    {
                        list.Add(x.ToString());
                    }
                    else
                    {
                        list.Add(ObjectToString(x));
                    }
                }
            }

            return string.Join(Environment.NewLine, list);
        }

        private string ObjectToString(object x)
        {
            var builder = new StringBuilder();
            builder.Append(x.GetType().Name);
            builder.Append("[");
            var properties = x.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var propertyValues = properties.Select(p => $"{p.Name}={p.GetValue(x) ?? "null"}");
            builder.Append(string.Join(", ", propertyValues));
            builder.Append("]");

            return builder.ToString();
        }
        
        private VerificationException GetMethodErrorMessageInverted(T result, Expression<Func<T, bool>> assertion, Expression expression)
        {
            var mce = expression as MethodCallExpression;
            if (mce == null)
            {
                throw new NotSupportedException("Expected method, got " + expression.Type.Name);
            }
            if (mce.Method.ReturnType != typeof(bool))
            {
                throw new NotSupportedException($"method verification must return bool, not {mce.Method.ReturnType}");
            }

            

            var actual = GetValue(result, expression);
            var names = "!" + string.Join(".", GetName(mce));
            if (!actual.Succeeded)
            {
                return new VerificationException($"Expected {names} to be false, but was true.") { Target = names };
            }

            if (actual.Enumerable != null)
            {
                var enumerableString = GetEnumerableString(actual.Enumerable);
                

                var message = $"Expected {names} to be true, but was false.";

                if (actual.Enumerable is string)
                {
                    return new VerificationException(message + $" String is <{enumerableString}>")
                    {
                        Target = names,
                        Enumerable = actual.Enumerable
                    };
                }
                return new VerificationException(message + $" Collection contains: {Environment.NewLine}{enumerableString}")
                {
                    Target = names,
                    Enumerable = actual.Enumerable
                };
            }

            return new VerificationException($"Tried to verify {names}, but {actual.NullPropertyName} was null")
            {
                Target = names
            };
        }

        private List<string> GetName(Expression expression)
        {
            var expressionStack = GetExpressionStack(expression, true);
            var result = new List<string>();
            while (expressionStack.Any())
            {
                var current = expressionStack.Pop();
                if (current is MemberExpression m)
                {
                    result.Add(m.Member.Name);
                } else if (current is MethodCallExpression mce)
                {
                    var arguments = string.Join(", ", mce.Arguments.Select(e => e.ToString()));
                    result.Add($"{mce.Method.Name}({arguments})");
                } else if (current is UnaryExpression u)
                {
                    if (u.NodeType == ExpressionType.Not)
                    {
                        result.Add("!");
                    }
                    else
                    {
                        result.Add(u.NodeType.ToString());
                    }
                }
            }

            return result;
        }

        private Stack<Expression> GetExpressionStack(Expression expression, bool forNames = false)
        {
            var result = new Stack<Expression>();
            var current = expression;
            while (true)
            {
                result.Push(current);
                if (current is MemberExpression m)
                {
                    current = m.Expression;
                } else if (current is MethodCallExpression met)
                {
                    
                    current = met.Object ?? (forNames ? met.Arguments.FirstOrDefault() : null);
                } else if (current is UnaryExpression u)
                {
                    current = u.Operand;
                }
                else
                {
                    break;
                }

                if (current is ParameterExpression)
                {
                    break;
                }
            }

            return result;
        }

        private ValueResult GetValue(object container, Expression expression)
        {
            var expressionStack = GetExpressionStack(expression);
            var current = container;
            while (expressionStack.Any())
            {
                var ex = expressionStack.Pop();
                if (ex is LambdaExpression l)
                {
                    return ValueResult.Ok(l.Compile());
                }
                else if (ex is MemberExpression m)
                {
                    if (current is ValueResult vr)
                    {
                        current = GetValue(vr.Value, m);
                    }
                    else
                    {
                        current = GetValue(current, m);
                    }
                    
                    if (current == null && expressionStack.Any())
                    {
                        return ValueResult.Null(m.Member.Name);
                    }
                    
                } else if (ex is MethodCallExpression met)
                {
                    current = GetValue(current, met);
                    
                    if (current == null && expressionStack.Any())
                    {
                        return ValueResult.Null(met.Method.Name);
                    }
                } 
                else if (ex is UnaryExpression u)
                {
                    if (u.NodeType != ExpressionType.Not)
                    {
                        throw new NotSupportedException($"Only unary expression with NOT is supported: {u.NodeType}");
                    }
                    
                    if (current is ValueResult vr)
                    {
                        current = new ValueResult
                        {
                            Value = !(bool)vr.Value,
                            Succeeded = !vr.Succeeded,
                            Enumerable = vr.Enumerable,
                            NullPropertyName = vr.NullPropertyName
                        };
                    }
                    else
                    {
                        current = !(bool)current;
                    }
                    
                }
                else if (expression is ConstantExpression c)
                {
                    current = c.Value;
                }
            }

            if (current is ValueResult r)
            {
                return r;
            }

            return ValueResult.Ok(current);
        }

        private object GetValue(object container, MemberExpression expression)
        {
            if (expression.Member is PropertyInfo p)
            {
                return p.GetValue(container);
            }

            if (expression.Member is FieldInfo f)
            {
                return f.GetValue(container);
            }

            return null;
        }

        private object GetValue(object container, MethodCallExpression expression)
        {
            var value = container is ValueResult vr ? vr.Value : container;
            var arguments = expression.Arguments.Select(e => GetValue(container, e)).Select(v => v.Value).ToArray();
            var result = expression.Method.Invoke(value, arguments);

            if (expression.Object == null && expression.Arguments.Any())
            {
                var thisParameter = expression.Arguments.FirstOrDefault();
                var thisValue = GetValue(container, thisParameter)?.Value;
                if (thisValue is IEnumerable extensionEn)
                {
                    return ValueResult.EnumerableResult(extensionEn, result);
                }
            }
            if (value is IEnumerable en)
            {
                return ValueResult.EnumerableResult(en, result);
            }
            
            return result;
        }
    }
}