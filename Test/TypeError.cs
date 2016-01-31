using System;
using System.Runtime.Serialization;

namespace Mint
{
    [Serializable]
    internal class TypeError : Exception
    {
        public TypeError()
        {
        }

        public TypeError(string message) : base(message)
        {
        }

        public TypeError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}