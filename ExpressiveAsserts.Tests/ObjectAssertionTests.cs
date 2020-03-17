using System.Collections.Generic;
using ExpressiveAsserts.Extensions;
using Xunit;

namespace ExpressiveAsserts.Tests
{
    public class ObjectAssertionTests
    {
        [Fact]
        public void CanAssertFailEqualityBasedOnObject()
        {
            var toTest = new PropertyTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations =
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var e = Assert.Throws<VerificationException>(() => toTest.Verify(() => new PropertyTest
            {
                Name = "Fooba"
            }));

            Assert.Equal(nameof(PropertyTest.Name), e.Target);
            Assert.Equal("Fooba", e.ExpectedValue);
            Assert.Equal("Foobar", e.ActualValue);
        }
        
        [Fact]
        public void CanAssertEqualityBasedOnObject()
        {
            var toTest = new PropertyTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations =
                {
                    "Europe",
                    "Asia"
                }
            };
            
            toTest.Verify(() => new PropertyTest
            {
                Name = "Foobar"
            });
        }

        [Fact]
        public void CanAssertEqualityFailBasedOnLocalDefinedObjectAndOnlyAssertNonDefaultProperties()
        {
            var toTest = new PropertyTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations =
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var expected = new PropertyTest
            {
                Name = "Fooba"
            };
            
            var e = Assert.Throws<VerificationException>(()=> toTest.Verify(() => expected));

            Assert.Equal(nameof(PropertyTest.Name), e.Target);
            Assert.Equal("Fooba", e.ExpectedValue);
            Assert.Equal("Foobar", e.ActualValue);
        }
        
        [Fact]
        public void CanAssertEqualityFailIndirectlyBasedOnLocalDefinedObject()
        {
            var toTest = new PropertyTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations =
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var expected = new PropertyTest
            {
                Name = "Fooba"
            };
            
            var e = Assert.Throws<VerificationException>(()=> toTest.Verify(p => p.Name == expected.Name));
            Assert.Equal("p.Name == expected.Name", e.Message);
        }
        
        [Fact]
        public void CanAssertEqualityFailBasedOnLocalDefinedObjectAndOnlyAssertNonDefaultFields()
        {
            var toTest = new FieldTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations = new List<string>
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var expected = new FieldTest
            {
                Name = "Fooba"
            };
            
            var e = Assert.Throws<VerificationException>(()=> toTest.Verify(() => expected));

            Assert.Equal(nameof(PropertyTest.Name), e.Target);
            Assert.Equal("Fooba", e.ExpectedValue);
            Assert.Equal("Foobar", e.ActualValue);
        }
        
        [Fact]
        public void CanAssertEqualityBasedOnLocalDefinedObjectAndOnlyAssertNonDefaultProperties()
        {
            var toTest = new PropertyTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations =
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var expected = new PropertyTest
            {
                Name = "Foobar"
            };
            
            toTest.Verify(() => expected);
        }
        
        [Fact]
        public void CanAssertEqualityBasedOnLocalDefinedObjectAndOnlyAssertNonDefaultFields()
        {
            var toTest = new FieldTest
            {
                Id = 7,
                Name = "Foobar",
                Age = 42,
                Locations = new List<string>
                {
                    "Europe",
                    "Asia"
                }
            };
            
            var expected = new FieldTest
            {
                Name = "Foobar"
            };
            
            toTest.Verify(() => expected);
        }

        
        
    }
    
    class PropertyTest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
            
        public List<string> Locations { get; set; } = new List<string>();
    }
        
    class FieldTest
    {
        public long Id;
        public string Name;
        public int Age;

        public List<string> Locations;
    }

    
}