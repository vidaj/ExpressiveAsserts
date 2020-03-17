namespace ExpressiveAsserts.TestCases
{
    public class SimpleTestCase<T> : TestCase<T>
    {
        public T ToTest { get; }

        public SimpleTestCase(T test)
        {
            ToTest = test;
        }
        
        public virtual void Run()
        {
            Verify(ToTest);
        }
    }
    
    public abstract class SimpleTestCase<T, Result> : TestCase<T>
    {
        public T ToTest { get; }

        public SimpleTestCase(T test)
        {
            ToTest = test;
        }

        public abstract Result Run();
    }
}