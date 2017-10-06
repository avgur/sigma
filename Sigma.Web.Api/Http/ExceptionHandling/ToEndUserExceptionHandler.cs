namespace Sigma.Web.Http.ExceptionHandling
{
    using System.Net;
    using System;
    using Sigma.Web.Http.Exceptions;

    public class ToEndUserExceptionHandler : ToEndUserExceptionHandler<Exception>
    {
        public ToEndUserExceptionHandler(HttpStatusCode httpStatusCode, string message)
            : base(httpStatusCode, message)
        {

        }
    }

    public class ToEndUserExceptionHandler<T> : Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.IExceptionHandler
        where T : Exception
    {
        private Func<T, string> errorCodeMapper;

        private Func<T, string> errorNumberMapper;

        private Func<T, string> messageMapper;

        private Func<T, HttpStatusCode> httpStatusCodeMapper;

        public ToEndUserExceptionHandler(HttpStatusCode httpStatusCode, string message)
        : this(
                httpStatusCode,
                httpStatusCode.ToString("G"),
                httpStatusCode.ToString("D"),
                message
                )
        {

        }

        public ToEndUserExceptionHandler(HttpStatusCode httpStatusCode, string errorCode, string errorNumber, string message = null)
            : this(
                ex => httpStatusCode,
                ex => errorCode,
                ex => errorNumber,
                ex => message)
        {
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrEmpty(errorCode));
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrEmpty(errorNumber));
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrEmpty(message));
        }

        public ToEndUserExceptionHandler(
            Func<T, HttpStatusCode> httpStatusCodeMapper = null,
            Func<T, string> errorCodeMapper = null,
            Func<T, string> errorNumberMapper = null,
            Func<T, string> messageMapper = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(httpStatusCodeMapper != null);
            System.Diagnostics.Contracts.Contract.Assert(errorCodeMapper != null);
            System.Diagnostics.Contracts.Contract.Assert(errorNumberMapper != null);
            this.errorCodeMapper = errorCodeMapper;
            this.errorNumberMapper = errorNumberMapper;
            this.httpStatusCodeMapper = httpStatusCodeMapper;
            this.messageMapper = messageMapper ?? (ex => ex.Message);
        }

        public virtual Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            var castException = exception as T;
            System.Diagnostics.Contracts.Contract.Assert(castException != null);

            var message = this.messageMapper(castException);
            var errorCode = this.errorCodeMapper(castException);
            var errorNumber = this.errorNumberMapper(castException);
            var statusCode = this.httpStatusCodeMapper(castException);
            var instance = new EndUserException(message, castException)
            {
                HttpStatusCode = statusCode,
                ErrorCode = errorCode,
                ErrorNumber = errorNumber,
                HandlingInstanceId = handlingInstanceId
            };

            return instance;
        }
    }
}