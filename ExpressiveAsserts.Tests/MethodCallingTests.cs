using System;
using ExpressiveAsserts.Extensions;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class MethodCallingTests
    {
        [Fact]
        public void CanCallMethodInAssert()
        {
            var toTest = new Test("heLLo");
            var e = Assert.Throws<VerificationException>(() =>toTest.Verify(t => t.Name.Equals("hell", StringComparison.CurrentCultureIgnoreCase)));
            Assert.Equal("Name.Equals(\"hell\", CurrentCultureIgnoreCase)", e.Target);
            Assert.Equal("heLLo", e.Enumerable);
        }

        [Fact]
        public void CanCallMethodDirectoryOnTestObject()
        {
            var toTest = "heLLo";
            var e = Assert.Throws<VerificationException>(() => toTest.Verify(p => p.Equals("hello")));
            Assert.Equal("Equals(\"hello\")", e.Target);
            Assert.Equal("heLLo", e.Enumerable);
        }
    }

    class Test
    {
        public string Name { get; set; }

        public Test(string name)
        {
            Name = name;
        }
    }
}