namespace Sigma.Web.Http.ExceptionHandling
{
    using System.Net;
    using System;

    public class UnexpectedErrorExceptionHandler : ToEndUserExceptionHandler<Exception>
    {
        public UnexpectedErrorExceptionHandler()
            : base(HttpStatusCode.InternalServerError, "Unexpected error occured. Please contact administrator for details")
        {
        }
    }
}