using ExpressiveAsserts.Extensions;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class ExtensionMethodTests
    {
        [Fact]
        public void CanAssertDirectlyViaExtensionMethod()
        {
            try
            {
                var toTest = new TestClass { Text = "foo" };
                toTest.Verify(t => t.Text == "foobar");
            }
            catch (VerificationException e)
            {
                Assert.Equal(nameof(TestClass.Text), e.Target);
                Assert.Equal("foobar", e.ExpectedValue);
                Assert.Equal("foo", e.ActualValue);
            }
        }
        
        [Fact]
        public void CanAssertFailMultipleAssertsDirectlyViaExtensionMethod()
        {
            try
            {
                var toTest = new TestClass { Text = "foo", Text2 = "bar" };
                toTest.Verify(t => t.Text == "foo", t => t.Text2 == "foobar");
            }
            catch (VerificationException e)
            {
                Assert.Equal(nameof(TestClass.Text2), e.Target);
                Assert.Equal("foobar", e.ExpectedValue);
                Assert.Equal("bar", e.ActualValue);
            }
        }
        
        [Fact]
        public void CanAssertMultipleAssertsDirectlyViaExtensionMethod()
        {
            var toTest = new TestClass { Text = "foo", Text2 = "bar" };
            toTest.Verify(
                t => t.Text == "foo", 
                t => t.Text2 == "bar");   
        }

        [Fact(Skip = "Not working yet")]
        public void CanDoMultipleAssertsWithAndInSingleExpression()
        {
            var toTest = new TestClass { Text = "foo", Text2 = "bar" };
            toTest.Verify(t => t.Text == "foo" && t.Text == "bar");
        }
    }

    class TestClass
    {
        public string Text { get; set; }
        
        public string Text2 { get; set; }
    }
}