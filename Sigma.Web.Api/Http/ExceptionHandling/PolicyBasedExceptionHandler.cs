namespace Sigma.Web.Http.ExceptionHandling
{
    using System.Web.Http;

    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Sigma.Web.Http.Exceptions;

    /// <summary>
    /// Transforms exceptions to user readable data
    /// </summary>
    public class PolicyBasedExceptionHandler : System.Web.Http.ExceptionHandling.ExceptionHandler
    {
        private readonly ExceptionManager exceptionManager;
        private readonly string policyName;
        private readonly Func<HttpRequestMessage, string> trackingIdFetcher;

        public PolicyBasedExceptionHandler(ExceptionManager exceptionManager, string policyName, Func<HttpRequestMessage, string> trackingIdFetcher)
        {
            this.policyName = policyName;
            this.exceptionManager = exceptionManager;
            this.trackingIdFetcher = trackingIdFetcher;
        }

        public override void Handle(System.Web.Http.ExceptionHandling.ExceptionHandlerContext context)
        {
            context.Result = new ErrorResult
            {
                ExceptionManager = this.exceptionManager,
                PolicyName = this.policyName,
                Context = context,
                TrackingIdFetcher = this.trackingIdFetcher
            };
        }

        private class ErrorResult : IHttpActionResult
        {
            public ExceptionManager ExceptionManager { get; set; }
            
            public string PolicyName{ get; set; }

            public System.Web.Http.ExceptionHandling.ExceptionHandlerContext Context { get; set; }

            public Func<HttpRequestMessage, string> TrackingIdFetcher { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var handled = this.ExceptionManager.HandleException(this.Context.Exception, this.PolicyName, out Exception exception);
                System.Diagnostics.Contracts.Contract.Assert(handled);
                var endUserException = exception as EndUserException;
                System.Diagnostics.Contracts.Contract.Assert(endUserException != null);

                
                var trackingId = this.TrackingIdFetcher(this.Context.Request) ?? endUserException.HandlingInstanceId.ToString("N");

                var data = new Dictionary<string, object>()
                {
                    { "TrackingId",  trackingId },
                    { "Date",  DateTime.UtcNow },
                    { "ErrorCode",  endUserException.ErrorCode },
                    { "ErrorNumber",  endUserException.ErrorNumber },
                    { "Message",  endUserException.Message }
                };

                if (endUserException.Errors != null)
                {
                    data.Add("Errors", endUserException.Errors);
                }

#if DEBUG
                data.Add("InnerException", endUserException.InnerException);
#endif

                var response = this.Context.Request.CreateResponse(
                    endUserException.HttpStatusCode,
                    data
                );

                return Task.FromResult(response);
            }
        }
    }
}