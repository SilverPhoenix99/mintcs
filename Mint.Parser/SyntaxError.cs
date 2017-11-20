using System;
using System.Collections;

namespace Mint
{
    public class SyntaxError : Exception
    {
        [NonSerialized]
        private IDictionary data;

        public override IDictionary Data => data ?? (data = new Hashtable());

        public SyntaxError(string message) : base(message)
        { }
        
        public SyntaxError(string message, Exception innerException) : base(message, innerException)
        { }
        
        public SyntaxError(string filename, int line, string message, Exception innerException = null)
            : this($"{filename}:{line}: {message}", innerException)
        { }
    }
}
