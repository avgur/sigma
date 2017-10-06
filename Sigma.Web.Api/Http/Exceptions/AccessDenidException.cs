
namespace Sigma.Web.Http.Exceptions
{
    using System;

    [Serializable]
    public class AccessDenidException : HttpRootException
    {
        public AccessDenidException() { }
        public AccessDenidException(string message) : base(message) { }
        public AccessDenidException(string message, Exception inner) : base(message, inner) { }
        protected AccessDenidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}