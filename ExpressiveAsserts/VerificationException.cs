using System;
using System.Collections;

namespace ExpressiveAsserts
{
    public class VerificationException : Exception
    {
        public object ExpectedValue { get; set; }
        
        public object ActualValue { get; set; }
        
        public string NullProperty { get; set; }
        
        public string Target { get; set; }
        public IEnumerable Enumerable { get; set; }

        public VerificationException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}