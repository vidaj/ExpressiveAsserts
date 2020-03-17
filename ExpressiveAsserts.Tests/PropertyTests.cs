using System;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void CanVerifyPropertyWithoutValue()
        {
            var toTest = new Properties();
            var error = new PropertyCase(toTest)
            {
                p => p.String == "foobar"
            }.Run();

            Assert.Equal("foobar", error.ExpectedValue);
            Assert.Null(error.ActualValue);
            Assert.Equal(nameof(Properties.String), error.Target);
        }
        
        [Fact]
        public void CanVerifyPropertyWithValue()
        {
            var toTest = new Properties
            {
                String = "foo"
            };
            var error = new PropertyCase(toTest)
            {
                p => p.String == "foobar"
            }.Run();

            Assert.Equal("foobar", error.ExpectedValue);
            Assert.Equal("foo", error.ActualValue);
            Assert.Equal(nameof(Properties.String), error.Target);
        }

        [Fact]
        public void CanVerifyMultipleProperties()
        {
            var toTest = new Properties
            {
                String = "foo",
                Long = 42
            };
            var error = new PropertyCase(toTest)
            {
                p => p.String == "foo",
                p => p.Long == 41
            }.Run();

            Assert.Equal(41L, error.ExpectedValue);
            Assert.Equal(42L, error.ActualValue);
            Assert.Equal(nameof(Properties.Long), error.Target);
        }

        [Fact]
        public void CanVerifyMultiplePropertiesWithoutFailing()
        {
            var now = DateTime.Now;
            var toTest = new Properties
            {
                String = "foo",
                Long = 42,
                Boolean = true,
                DateTime = now
            };
            var error = new PropertyCase(toTest)
            {
                p => p.String == "foo",
                p => p.Long == 42,
                p => p.Boolean == true,
                p => p.DateTime == now
            }.Run();

            Assert.Null(error);
        }
    }

    class Properties
    {
        public string String { get; set; }
        
        public bool Boolean { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public long Long { get; set; }
    }

    class PropertyCase : ExceptionCatcherTestCase<Properties>
    {
        public PropertyCase(Properties test) : base(test)
        {
        }
    }
}