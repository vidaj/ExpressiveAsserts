using System.Collections;

namespace ExpressiveAsserts
{
    public class ValueResult
    {
        public object Value { get; set; }
        public string NullPropertyName { get; set; }
        public bool Succeeded { get; set; }
        
        public IEnumerable Enumerable { get; set; }

        public ValueResult()
        {
            
        }

        private ValueResult(object value)
        {
            Value = value;
            Succeeded = true;
        }

        private ValueResult(string nullPropertyName)
        {
            NullPropertyName = nullPropertyName;
            Succeeded = false;
        }

        private ValueResult(IEnumerable enumerable, object value, bool succeeded)
        {
            Enumerable = enumerable;
            Value = value;
            Succeeded = succeeded;
        }

        public static ValueResult Ok(object value)
        {
            return new ValueResult(value);
        }

        public static ValueResult Null(string nullName)
        {
            return new ValueResult(nullName);
        }

        public static ValueResult EnumerableResult(IEnumerable enumerable, object value)
        {
            var succeeded = (value is ValueResult vr && (bool)vr.Value) || (value is bool b && b);
            return new ValueResult(enumerable, value, succeeded);
        }

        public override string ToString()
        {
            return Succeeded ? (Value == null ? "null" : Value.ToString()) : NullPropertyName;
        }
    }
}