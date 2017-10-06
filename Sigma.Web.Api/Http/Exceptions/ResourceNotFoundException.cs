
namespace Sigma.Web.Http.Exceptions
{
    using System;

    [Serializable]
    public class ResourceNotFoundException : HttpRootException
    {
        public ResourceNotFoundException() { }
        public ResourceNotFoundException(string message) : base(message) { }
        public ResourceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ResourceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}