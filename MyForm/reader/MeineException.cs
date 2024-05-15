using System;
using System.Runtime.Serialization;

namespace MyForm.reader
{
    [Serializable]
    internal class MeineException : Exception
    {
        public MeineException()
        {
        }

        public MeineException(string message) : base(message)
        {
        }

        public MeineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MeineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}