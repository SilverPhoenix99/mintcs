using System;
using System.Runtime.Serialization;

namespace Mint
{
    [Serializable]
    public class ArgumentError : Exception
    {
        public ArgumentError()
        { }

        public ArgumentError(string message) : base(message)
        { }

        public ArgumentError(string message, Exception innerException) : base(message, innerException)
        { }

        protected ArgumentError(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
