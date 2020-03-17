using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class ExpressionStringBuilderTest
    {
        private int one = 1;
        private int two = 2;
        private string foo = "foo";
        private string bar = "bar";
        private string foobar = "foobar";
        private bool True = true;
        
        [Fact]
        public void Add()
        {
            Test(() => one + two, "one + two");
        }

        [Fact]
        public void Subtract()
        {
            Test(() => one - two, "one - two");
        }

        [Fact]
        public void Equal()
        {
            Test(() => one == two, "one == two");
        }

        [Fact]
        public void NotEqual()
        {
            Test(() => one != two, "one != two");
        }

        [Fact]
        public void GreaterThan()
        {
            Test(() => one > two, "one > two");
        }

        [Fact]
        public void GreaterThanEqual()
        {
            Test(() => one >= two, "one >= two");
        }

        [Fact]
        public void LesserThan()
        {
            Test(() => one < two, "one < two");
        }

        [Fact]
        public void LesserThanEqual()
        {
            Test(() => one <= two, "one <= two");
        }

        [Fact]
        public void Multiply()
        {
            Test(() => one * two, "one * two");
        }

        [Fact]
        public void Divide()
        {
            Test(() => one / two, "one / two");
        }

        [Fact]
        public void Modulo()
        {
            Test(() => one % two, "one % two");
        }

        [Fact]
        public void StringLiteralQuoted()
        {
            Test(() => "foo", "\"foo\"");
        }

        [Fact]
        public void StringEquals()
        {
            Test(() => foo != "bar", "foo != \"bar\"");
        }

        [Fact]
        public void ObjectProperty()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Name == "foo", "o.Name == \"foo\"");
        }

        [Fact]
        public void SingleLayerNestedObjectProperty()
        {
            var o = new NestedObjectPropertyStringTest();
            Test(() => o.Nested.Name == "foo", "o.Nested.Name == \"foo\"");
        }
        
        [Fact]
        public void MultiLayerNestedObjectProperty()
        {
            var o = new NestedObjectPropertyStringTest();
            Test(() => o.Nested.Nested.Name == "foo", "o.Nested.Nested.Name == \"foo\"");
        }

        [Fact]
        public void MethodCallWithoutArguments()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Create(), "o.Create()");
        }

        [Fact]
        public void MethodCallAndOperator()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Create() == foo, "o.Create() == foo");
        }

        [Fact]
        public void MethodCallWithStringLiteralArgument()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Create("foo") == foo, "o.Create(\"foo\") == foo");
        }

        [Fact]
        public void MethodCallWithNumberLiteralArgument()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Number(42), "o.Number(42)");
        }
        
        [Fact]
        public void MethodCallWithMultipleArguments()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.CreateTwo("foo", bar) == foo, "o.CreateTwo(\"foo\", bar) == foo");
        }

        [Fact]
        public void MethodCallWithVarArgs()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.CreateParams("foo", foo, bar), "o.CreateParams(\"foo\", foo, bar)");
        }
        
        [Fact]
        public void MethodCallWithNormalParametersAndVarArgs()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.MixedParams(one, "foo", foo, bar), "o.MixedParams(one, \"foo\", foo, bar)");
        }

        [Fact]
        public void MultipleMethodCallsWithAnd()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Test() && o.Test() == true, "o.Test() && o.Test() == true");
        }

        [Fact]
        public void MethodCallWithLambda()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Lambda(() => one + 2), "o.Lambda(() => one + 2)");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithSingleParameter()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Lambda(f => one + 2), "o.Lambda(f => one + 2)");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithMultipleParameter()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Lambda((f, x) => one + 2), "o.Lambda((f, x) => one + 2)");
        }
        
        [Fact]
        public void MethodCallWithActionParameter()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.Action(() => new SimpleObjectPropertyStringTest()), "o.Action(() => new SimpleObjectPropertyStringTest())");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithNew()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.ComplexLambda(() => new SimpleObjectPropertyStringTest()), "o.ComplexLambda(() => new SimpleObjectPropertyStringTest())");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithNewAndConstructorParameters()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.ComplexLambda(() => new SimpleObjectPropertyStringTest("foo", one)), "o.ComplexLambda(() => new SimpleObjectPropertyStringTest(\"foo\", one))");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithNewAndPropertyInitialization()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.ComplexLambda(() => new SimpleObjectPropertyStringTest { Id = 42}), "o.ComplexLambda(() => new SimpleObjectPropertyStringTest() { Id = 42 })");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithNewAndNegatedReferenceValueAsConstructorArgument()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.ComplexLambda(() => new SimpleObjectPropertyStringTest(!True)), "o.ComplexLambda(() => new SimpleObjectPropertyStringTest(!True))");
        }
        
        [Fact]
        public void MethodCallWithLambdaWithNewAndPropertyInitializationWithNegatedReference()
        {
            var o = new SimpleObjectPropertyStringTest();
            Test(() => o.ComplexLambda(() => new SimpleObjectPropertyStringTest { Id = -one}), "o.ComplexLambda(() => new SimpleObjectPropertyStringTest() { Id = -one })");
        }

        private void Test<T>(Expression<Func<T>> e, string expected)
        {
            var result = Create(e);
            Assert.Equal(expected, result);
        }
        
        private void Test(Expression<Action> e, string expected)
        {
            var result = Create(e);
            Assert.Equal(expected, result);
        }
        
        private string Create<T>(Expression<Func<T>> e)
        {
            return new ExpressionStringBuilder().Build(e);
        }
        
        private string Create(Expression<Action> e)
        {
            return new ExpressionStringBuilder().Build(e);
        }
    }

    class SimpleObjectPropertyStringTest
    {
        public string Name { get; set; }
        
        public long Id { get; set; }
        
        public List<string> List { get; set; }
        
        public bool Bool { get; set; }


        public SimpleObjectPropertyStringTest(bool b)
        {
            
        }
        
        public SimpleObjectPropertyStringTest(string name, long id)
        {
            
        }

        public SimpleObjectPropertyStringTest()
        {
            
        }

        public string Create()
        {
            return "";
        }

        public string Create(string foo)
        {
            return foo;
        }
        
        public string CreateTwo(string foo, string bar)
        {
            return foo;
        }

        public string CreateParams(params string[] foo)
        {
            return "";
        }
        
        public string MixedParams(int number, params string[] foo)
        {
            return "";
        }

        public string Number(int number)
        {
            return "";
        }

        public string Lambda(Func<int> func)
        {
            return "";
        }
        
        public string Lambda(Func<int, int> func)
        {
            return "";
        }
        
        public string Lambda(Func<int, int, int> func)
        {
            return "";
        }

        public void Action(Action a)
        {
            
        }

        public bool Test()
        {
            return false;
        }

        public void ComplexLambda(Func<SimpleObjectPropertyStringTest> fun)
        {
            
        } 
    }

    class NestedObjectPropertyStringTest : SimpleObjectPropertyStringTest
    {
        public NestedObjectPropertyStringTest Nested { get; set; }
    }
}