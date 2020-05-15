using System;
using System.Runtime.Serialization;

namespace CoviIDApiCore.Exceptions
{
    [Serializable]
    public class BitlyException : Exception
    {
        public BitlyException()
        {
        }

        public BitlyException(string message) : base(message)
        {
        }

        public BitlyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BitlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}