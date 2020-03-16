using System;

namespace ExpressiveAsserts
{
    public class VerificationException : Exception
    {
        public VerificationException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}