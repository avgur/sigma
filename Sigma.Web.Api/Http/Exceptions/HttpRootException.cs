namespace Sigma.Web.Http.Exceptions
{
    using System;

    [Serializable]
    public class HttpRootException : Exception
    {
        public HttpRootException() { }
        public HttpRootException(string message) : base(message) { }
        public HttpRootException(string message, Exception inner) : base(message, inner) { }
        protected HttpRootException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}