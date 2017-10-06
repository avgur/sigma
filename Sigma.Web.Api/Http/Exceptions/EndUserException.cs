namespace Sigma.Web.Http.Exceptions
{
    using System;
    using System.Net;

    [Serializable]
    public class EndUserException : HttpRootException
    {
        public EndUserException() : this("Unexpected error occured. Please, contact administrator") { }
        public EndUserException(Exception inner) : this("Unexpected error occured. Please, contact administrator", inner) { }
        public EndUserException(string message) : this(message, null) { }
        public EndUserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EndUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public Guid HandlingInstanceId { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorNumber { get; set; }

        public OperationErrorEntry[] Errors { get; set; }

        public class OperationErrorEntry
        {
            public string Code { get; set; }

            public string Message { get; set; }
            public Exception Exception { get; internal set; }
        }
    }
}