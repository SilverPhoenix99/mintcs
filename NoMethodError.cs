using System;
using System.Runtime.Serialization;

namespace mint
{
    [Serializable]
    internal class NoMethodError : Exception
    {
        public NoMethodError()
        {
        }

        public NoMethodError(string message) : base(message)
        {
        }

        public NoMethodError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoMethodError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}