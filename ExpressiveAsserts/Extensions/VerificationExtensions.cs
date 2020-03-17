using System;
using System.Linq.Expressions;
using ExpressiveAsserts.TestCases;

namespace ExpressiveAsserts.Extensions
{
    public static class VerificationExtensions
    {
        public static void Verify<T>(this T o, params Expression<Func<T, bool>>[] assertions)
        {
            var testCase = new SimpleTestCase<T>(o);
            foreach (var assertion in assertions)
            {
                testCase.Add(assertion);
            }
            testCase.Run();
        }
        
        public static void Verify<T>(this T o, params Expression<Func<T>>[] assertions)
        {
            var testCase = new SimpleTestCase<T>(o);
            foreach (var assertion in assertions)
            {
                testCase.Add(assertion);
            }
            testCase.Run();
        }
    }
}