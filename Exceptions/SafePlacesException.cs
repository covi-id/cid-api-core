using System;
using System.Runtime.Serialization;

namespace CoviIDApiCore.Exceptions
{
    [Serializable]
    internal class SafePlacesException : Exception
    {
        public SafePlacesException()
        {
        }

        public SafePlacesException(string message) : base(message)
        {
        }

        public SafePlacesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SafePlacesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}