using ExpressiveAsserts.TestCases;

namespace ExpressiveAsserts.Tests
{
    public class ExceptionCatcherTestCase<T> : SimpleTestCase<T, VerificationException>
    {
        public ExceptionCatcherTestCase(T test) : base(test)
        {
        }

        public override VerificationException Run()
        {
            try
            {
                Verify(ToTest);
                return null;
            }
            catch(VerificationException e)
            {
                return e;
            }
        }
    }
}