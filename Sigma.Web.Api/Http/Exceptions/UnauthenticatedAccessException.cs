namespace Sigma.Web.Http.Exceptions
{
    using System;

    [Serializable]
    public class UnauthenticatedAccessException : HttpRootException
    {
        public UnauthenticatedAccessException() { }
        public UnauthenticatedAccessException(string message) : base(message) { }
        public UnauthenticatedAccessException(string message, Exception inner) : base(message, inner) { }
        protected UnauthenticatedAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}