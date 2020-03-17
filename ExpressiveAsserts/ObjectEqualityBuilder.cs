using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveAsserts
{
    public class ObjectEqualityBuilder
    {
        public VerificationException AreEqual<T>(T toTest, Expression<Func<T>> assertion)
        {
            var expectedObject = assertion.Compile()();

            if (assertion.Body is MemberInitExpression m)
            {
                return AssertEqual(toTest, expectedObject, m);    
            }

            if (assertion.Body is ConstantExpression c)
            {
                return AssertEqual(toTest, expectedObject);
            }

            if (assertion.Body is MemberExpression me)
            {
                return AssertEqual(toTest, expectedObject);
            }

            return null;
        }

        public VerificationException AssertEqual<T>(T actual, T expected)
        {
            if (typeof(T).IsValueType)
            {
                return AssertEquals(expected, actual, typeof(T).Name, typeof(T));
            }
            
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var expectedValue = property.GetValue(expected);
                if (ShouldEvaluateValue(expectedValue, property.PropertyType))
                {
                    var actualValue = property.GetValue(actual);
                    var result = AssertEquals(expectedValue, actualValue, property.Name, property.PropertyType);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var expectedValue = field.GetValue(expected);
                if (ShouldEvaluateValue(expectedValue, field.FieldType))
                {
                    var actualValue = field.GetValue(actual);
                    var result = AssertEquals(expectedValue, actualValue, field.Name, field.FieldType);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static bool ShouldEvaluateValue(object expectedValue, Type type)
        {
            var defaultValue = GetDefaultValue(type);
            if (Object.Equals(expectedValue, defaultValue))
            {
                return false;
            }

            if (IsEmptyEnumerable(expectedValue))
            {
                return false;
            }

            return true;
        }

        private static bool IsEmptyEnumerable(object expectedValue)
        {
            if (expectedValue is IEnumerable e)
            {
                return !e.GetEnumerator().MoveNext();
            }

            return false;
        }

        private VerificationException AssertEqual<T>(T actual, T expected, MemberInitExpression e)
        {
            foreach (var binding in e.Bindings)
            {
                var result = AssertEqual(actual, expected, binding);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private VerificationException AssertEqual<T>(T actual, T expected, MemberBinding binding)
        {
            var member = binding.Member;
            if (member is PropertyInfo p)
            {
                var result = AssertProperty(actual, expected, p);
                if (result != null)
                {
                    return result;
                }
            } else if (member is FieldInfo f)
            {
                var result = AssertField(actual, expected, f);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private VerificationException AssertField<T>(T actual, T expected, FieldInfo field)
        {
            var expectedValue = field.GetValue(expected);
            var actualValue = field.GetValue(actual);
            return AssertEquals(expectedValue, actualValue, field.Name, field.FieldType);
        }

        private static VerificationException AssertProperty<T>(T actual, T expected, PropertyInfo property)
        {
            var expectedValue = property.GetValue(expected);
            var actualValue = property.GetValue(actual);
            return AssertEquals(expectedValue, actualValue, property.Name, property.PropertyType);
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        private static string DefaultValueString(object o)
        {
            return o?.ToString() ?? "null";
        }

        private static VerificationException AssertEquals(object expectedValue, object actualValue, string name, Type type)
        {
            var defaultValue = GetDefaultValue(type);
            if (expectedValue == defaultValue && actualValue == defaultValue)
            {
                return null;
            }

            var nullString = DefaultValueString(defaultValue);

            string message = null;
            
            if (expectedValue == defaultValue)
            {
                message = $"Expected {name} to be {nullString}. Was '{actualValue}'.";
                
            }           
            else if (actualValue == defaultValue)
            {
                message = $"Expected {name} to be '{expectedValue}', but it was {defaultValue}.";
            }
            else if (!expectedValue.Equals(actualValue))
            {
                message = $"Expected {name} to be '{expectedValue}', but it was '{actualValue}'";
            }

            if (message != null)
            {
                return new VerificationException(message)
                {
                    Target = name,
                    ActualValue = actualValue,
                    ExpectedValue = expectedValue
                };
            }

            return null;
        }
    }
}