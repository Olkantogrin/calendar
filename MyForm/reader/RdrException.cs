using System;
using System.Runtime.Serialization;

namespace MyForm.reader
{
    [Serializable]
    internal class RdrException : Exception
    {
        public RdrException()
        {
        }

        public RdrException(string message) : base(message)
        {
        }

        public RdrException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RdrException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}