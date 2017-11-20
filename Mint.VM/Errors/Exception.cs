using System;
using System.Collections;

namespace Mint
{
    public class Exception : System.Exception
    {
        [NonSerialized]
        private IDictionary data;

        public override IDictionary Data => data ?? (data = new Hashtable());

        public Exception()
        { }
        
        public Exception(string message) : base(message)
        { }
        
        public Exception(string message, System.Exception innerException) : base(message, innerException)
        { }
    }
}