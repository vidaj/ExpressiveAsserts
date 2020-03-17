using ExpressiveAsserts.Extensions;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class ValueTypeTests
    {
        [Fact]
        public void CanAssertEqualityFailDirectlyOnInt()
        {
            var e = Assert.Throws<VerificationException>(() => 42.Verify(() => 41));
            Assert.Equal(41, e.ExpectedValue);
            Assert.Equal(42, e.ActualValue);
            Assert.Equal(typeof(int).Name, e.Target);
        }
        
        [Fact]
        public void CanAssertEqualityFailIndirectlyOnInt()
        {
            var e = Assert.Throws<VerificationException>(() => 42.Verify(p => p == 41));
            Assert.Equal(41, e.ExpectedValue);
            Assert.Equal(42, e.ActualValue);
            Assert.Equal(typeof(int).Name, e.Target);
        }
        
        [Fact]
        public void CanAssertEqualityDirectlyOnInt()
        {
            42.Verify(() => 42);
        }
        
        [Fact]
        public void CanAssertEqualityIndirectlyOnInt()
        {
            42.Verify(p => p == 42);
        }
        
        [Fact]
        public void CanAssertEqualityFailOnLocalVariableInt()
        {
            var myNumber = 41;
            var e = Assert.Throws<VerificationException>(() => 42.Verify(() => myNumber));
            Assert.Equal(41, e.ExpectedValue);
            Assert.Equal(42, e.ActualValue);
            Assert.Equal(typeof(int).Name, e.Target);
        }
        
        [Fact]
        public void CanAssertEqualityOnLocalVariableInt()
        {
            var myNumber = 42;
            42.Verify(() => myNumber);
        }
        
        [Fact]
        public void CanAssertEqualityFailDirectlyOnBool()
        {
            var e = Assert.Throws<VerificationException>(() => true.Verify(() => false));
            Assert.Equal(false, e.ExpectedValue);
            Assert.Equal(true, e.ActualValue);
            Assert.Equal(typeof(bool).Name, e.Target);
        }
        
        [Fact]
        public void CanAssertEqualityFailIndirectlyOnBool()
        {
            var e = Assert.Throws<VerificationException>(() => true.Verify(p => p == false));
            Assert.Equal(false, e.ExpectedValue);
            Assert.Equal(true, e.ActualValue);
            Assert.Equal(typeof(bool).Name, e.Target);
        }
        
        [Fact]
        public void CanAssertEqualityDirectlyOnBool()
        {
            true.Verify(() => true);
        }

        [Fact]
        public void CanAssertEqualityIndirectlyOnBool()
        {
            true.Verify(t => t == true);
        }
        
        [Fact]
        public void CanAssertEqualityFailOnLocalVariableBool()
        {
            var myTruth = false;
            var e = Assert.Throws<VerificationException>(() => true.Verify(() => myTruth));
            Assert.Equal(false, e.ExpectedValue);
            Assert.Equal(true, e.ActualValue);
            Assert.Equal(typeof(bool).Name, e.Target);
        }
        
        
        [Fact]
        public void CanAssertEqualityOnLocalVariableBool()
        {
            var myTruth = true;
            true.Verify(() => myTruth);
        }
    }
}