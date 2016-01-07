using System;
using System.Runtime.Serialization;

namespace mint
{
    [Serializable]
    internal class NameError : Exception
    {
        public NameError()
        {
        }

        public NameError(string message) : base(message)
        {
        }

        public NameError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NameError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}